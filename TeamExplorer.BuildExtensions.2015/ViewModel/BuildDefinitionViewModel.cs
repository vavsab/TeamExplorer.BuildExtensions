using System.Collections.ObjectModel;
using BuildTree.Models;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildTree.ViewModel
{
	public class BuildDefinitionViewModel : ViewModelBase
    {
        public ObservableCollection<BuildDefinitionViewModel> Children { get; set; }
        public BuildDefinitionViewModel Parent { get; set; }
        public string Name { get; set; }
        public bool IsBuildNode
        {
            get { return Definition != null; }
        }
        public IBuildDefinition Definition { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public BuildDefinitionViewModel(BuildDefinitionTreeNode node)
        {
            Name = node.Name;
            
            Children = new ObservableCollection<BuildDefinitionViewModel>();

            if (node.Children != null && node.Children.Count >0)
            {
                foreach (var child in node.Children)
                {
                    Children.Add(new BuildDefinitionViewModel(child, this));
                }
            }
            else 
            {
                Definition = node.BuildDefinition;
            }
        }
        
        private BuildDefinitionViewModel(BuildDefinitionTreeNode node, BuildDefinitionViewModel parent) : this(node)
        {
            Parent = parent;
        }
    }
}
