namespace hyperactive {
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class BranchViewModel : ViewModel {
        private string name;
        public string Name { get => name; private set => SetProperty(ref name, value); }

        private IEnumerable<TreeEntry> currentTree;
        public IEnumerable<TreeEntry> CurrentTree { get => currentTree; private set => SetProperty(ref currentTree, value); }

        public BranchViewModel(Branch branch) {
            Name = branch.FriendlyName;
            CurrentTree = branch.Tip.Tree;
        }
    }
}
