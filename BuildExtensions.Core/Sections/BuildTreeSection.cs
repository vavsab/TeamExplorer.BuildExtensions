using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using BuildTree.DialogCloser.Service;
using BuildTree.DialogCloser.Service.WindowViewModelMapping;
using BuildTree.DialogCloser.ViewModel;
using BuildTree.Models;
using BuildTree.ViewModel;
using BuildTree.Views;
using EnvDTE;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Controls;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamFoundation.Build;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BuildTree.Sections
{
    [TeamExplorerSection(BuildTreeSection.SectionId, TeamExplorerPageIds.Builds, 250)]
    public class BuildTreeSection : TeamExplorerBaseSection
    {
        public const string SectionId = "0C1852A7-0C9B-4D95-8893-02C60BEE271E";

		private OptionsViewModel _optionsViewModel;
		private readonly IDialogService _dialogService;

        private IVsTeamFoundationBuild _buildService;

        private ObservableCollection<BuildDefinitionViewModel> _builds = new ObservableCollection<BuildDefinitionViewModel>();
        public ObservableCollection<BuildDefinitionViewModel> Builds
        {
            get { return _builds; }
            protected set
            {
                this._builds = value;
                this.RaisePropertyChanged("Builds");
            }
        }

        public BuildDefinitionViewModel SelectedBuildDefinition { get; set; }

        protected BuildTreeView View
        {
            get { return this.SectionContent as BuildTreeView; }
        }

        public BuildTreeSection()
        {
            Title = "Build Tree";
            IsVisible = true;
            IsExpanded = true;
            IsBusy = false;
            SectionContent = new BuildTreeView();
            View.ParentSection = this;
			_optionsViewModel = new OptionsViewModel();
	        _dialogService = new DialogService(new WindowViewModelMappings());//ServiceLocator.Resolve<IDialogService>();
        }

        private DTE _dte = null;
        private Events _dteEvents = null;
        private DocumentEvents _dteDocumentEvents = null;
        public async override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            _buildService = GetService<IVsTeamFoundationBuild>();

            _dte = GetService<DTE>();            
            _dteEvents = _dte.Events;
            _dteDocumentEvents = _dte.Events.DocumentEvents;

            _dteDocumentEvents.DocumentSaved += _dteDocumentEvents_DocumentSaved;

			_optionsViewModel.OptionsModel.Initialize(this.ServiceProvider);

            var sectionContext = e.Context as BuildsSectionContext;
            if (sectionContext != null)
            {
                Builds = sectionContext.Builds;
            }
            else
            {
                await RefreshAsync();
            }
        }

        void _dteDocumentEvents_DocumentSaved(Document Document)
        {
            string documentTitle = Document.FullName;

            if (documentTitle.Contains("/Build/Definition"))
                Refresh();
        }

        public async override void Refresh()
        {
            base.Refresh();
            await this.RefreshAsync();
        }
        
        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            var context = new BuildsSectionContext {Builds = this.Builds};
            e.Context = context;
        }

        protected override async void ContextChanged(object sender, ContextChangedEventArgs e)
        {
            base.ContextChanged(sender, e);

            if (e.TeamProjectCollectionChanged || e.TeamProjectChanged)
            {
                await this.RefreshAsync();
            }
        }
        
        private async Task RefreshAsync()
        {
            try
            {
                this.IsBusy = true;
                this.Builds.Clear();

                var buildRefresh = new ObservableCollection<BuildDefinitionViewModel>();

				await Task.Run(() =>
				{
                    ITeamFoundationContext context = this.CurrentContext;
                    if (context != null && context.HasCollection && context.HasTeamProject)
                    {
                        var buildServer = context.TeamProjectCollection.GetService<IBuildServer>();
                        if (buildServer != null)
                        {
                            var buildDefinitions = buildServer.QueryBuildDefinitions(context.TeamProjectName);
                            var builDefinitionTree = BuildDefinitionTreeBuilder.Build(buildDefinitions,
		                        _optionsViewModel.OptionsModel.SplitCharacter.HasValue
			                        ? _optionsViewModel.OptionsModel.SplitCharacter.Value
			                        : '.');
                            foreach (var rootNode in builDefinitionTree)
                            {
                                buildRefresh.Add(new BuildDefinitionViewModel(rootNode));
                            }
                        }
                    }
                });

                this.Builds = buildRefresh;
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
            finally
            {
                this.IsBusy = false;
            }
        }

	    public void ShowSettings()
	    {
			try
			{
				var dlg = new OptionsViewModel(this._optionsViewModel);

				// It is important to either:
				// 1> Use the InitDialogInputData methode here or
				// 2> Reset the WindowCloseResult=null property
				// because otherwise ShowDialog will not work twice
				// (Symptom: The dialog is closed immeditialy by the attached behaviour)
				dlg.InitDialogInputData();

				// Showing the dialog, alternative 1.
				// Showing a specified dialog. This doesn't require any form of mapping using 
				// IWindowViewModelMappings.
				_dialogService.ShowDialog(this, dlg);

				// Copy input if user OK'ed it. This could also be done by a method, equality operator, or copy constructor
				if (dlg.OpenCloseView.WindowCloseResult == true)
				{
					_optionsViewModel.SplitCharacter = dlg.SplitCharacter;
					_optionsViewModel.OptionsModel.SaveSettings();
					Refresh();
				}
				else
					Console.WriteLine("Dialog was Cancelled.");
			}
			catch (Exception exc)
			{
				MessageBox.Show(exc.ToString());
			}
		}

        public void EditBuildDefinition()
        {
            if (SelectedBuildDefinition == null || SelectedBuildDefinition.Definition == null) return;

            if (_buildService != null)
            {
                _buildService.DefinitionManager.OpenDefinition(SelectedBuildDefinition.Definition.Uri);
            }
        }

        public void ViewBuilds()
        {
            if (SelectedBuildDefinition == null || SelectedBuildDefinition.Definition == null) return;

            if (_buildService != null)
            {
                _buildService.BuildExplorer.CompletedView.Show(SelectedBuildDefinition.Definition.TeamProject, SelectedBuildDefinition.Definition.Uri.AbsoluteUri, String.Empty, DateFilter.Today);
            }
        }

        public void QueueNewBuild()
        {
            if (SelectedBuildDefinition == null || SelectedBuildDefinition.Definition == null) return;

            if (_buildService != null)
            {
                _buildService.DetailsManager.QueueBuild(SelectedBuildDefinition.Definition.TeamProject, SelectedBuildDefinition.Definition.Uri);
            }
        }
	}
}
