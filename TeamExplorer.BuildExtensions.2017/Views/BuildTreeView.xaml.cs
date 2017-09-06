using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BuildTree.Sections;
using BuildTree.ViewModel;

namespace BuildTree.Views
{
    /// <summary>
    /// Interaction logic for BuildTreeView.xaml
    /// </summary>
    public partial class BuildTreeView : UserControl
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(BuildTreeSection), typeof(BuildTreeView));

        public BuildTreeSection ParentSection
        {
            get
            {
                return (BuildTreeSection)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
            }
        }

        public BuildTreeView()
        {
            InitializeComponent();
        }


	    private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	    {
			var item = sender as TreeViewItem;
			if (item != null)
			{
				item.Focus();
				e.Handled = true;

				var viewModel = item.DataContext as BuildDefinitionViewModel;
				if (viewModel != null)
				{
					ParentSection.SelectedBuildDefinition = viewModel;
					ParentSection.EditBuildDefinition();
				}
			}
		}

	    private void TreeViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                var viewModel = item.DataContext as BuildDefinitionViewModel;
                if (viewModel != null)
                {
                    ParentSection.SelectedBuildDefinition = viewModel;
                }

                item.Focus();

                if (item.ContextMenu != null && viewModel != null)
                {
	                item.ContextMenu.PlacementTarget = item;
                    item.ContextMenu.IsOpen = true;
				}

                e.Handled = true;
            }
        }

	    private static bool SafeIsBuildNode(object o)
	    {
		    var result = false;
		    var tv = o as BuildTreeView;
		    if (tv != null)
		    {
			    var vm = tv.TreeViewControl.SelectedItem as BuildDefinitionViewModel;
			    if (vm != null)
			    {
				    result = vm.IsBuildNode;
			    }
		    }
		    return result;
	    }

		private void ChangeSettingsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ViewBuildsCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = SafeIsBuildNode(sender);
		}

		private void QueueNewBuildCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = SafeIsBuildNode(sender);
		}

		private void EditBuildDefinitionCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = SafeIsBuildNode(sender);
		}

		private void ViewBuildsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			ParentSection.ViewBuilds();
		}

		private void QueueNewBuildCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			ParentSection.QueueNewBuild();
		}

		private void EditBuildDefinitionCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			ParentSection.EditBuildDefinition();
		}

		private void ChangeSettingsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			ParentSection.ShowSettings();
		}
	}

	public static class CustomCommands
	{
		public static readonly RoutedCommand ChangeSettingsCommand = new RoutedCommand
			(
					"ChangeSettingsCommand",
					typeof(CustomCommands)
			);
		public static readonly RoutedCommand ViewBuildsCommand = new RoutedCommand
			(
					"ViewBuilds",
					typeof(CustomCommands)
			);
		public static readonly RoutedCommand QueueNewBuildCommand = new RoutedCommand
			(
					"QueueNewBuild",
					typeof(CustomCommands)
			);
		public static readonly RoutedCommand EditBuildDefinitionCommand = new RoutedCommand
			(
					"EditBuildDefinition",
					typeof(CustomCommands)
			);
	}

}
