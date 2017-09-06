using System.Collections.ObjectModel;
using BuildTree.ViewModel;

namespace BuildTree.Sections
{
    internal class BuildsSectionContext
    {
        public ObservableCollection<BuildDefinitionViewModel> Builds { get; set; }
    }
}
