namespace hyperactive {
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class BranchViewModel : ViewModel {
        private string name;
        public string Name { get => name; private set => SetProperty(ref name, value); }

        private TreeEntry[] currentTree;
        public TreeEntry[] CurrentTree { get => currentTree; private set => SetProperty(ref currentTree, value); }

        public BranchViewModel(Branch branch) {
            Name = branch.FriendlyName;
            CurrentTree = branch.Tip.Tree
                .OrderBy(x => x, Comparer<TreeEntry>.Create(DirectoriesFirst))
                .ToArray();
        }

        private int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _) => -1,
            (_, Mode.Directory) => 1,
            (_, _) => a.Name.CompareTo(b.Name)
        };
    }
}
