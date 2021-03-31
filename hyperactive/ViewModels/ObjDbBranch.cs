namespace hyperactive {
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public class ObjDbBranch : ViewModel, IBranch {
        private readonly Tree repoRootTree;

        public string Name { get; }

        public bool IsHead { get; }

        private IDirectoryItem[] currentDirectory;
        public IDirectoryItem[] CurrentDirectory { get => currentDirectory; private set => SetProperty(ref currentDirectory, value); }

        private IDirectoryItem? selectedItem;
        public IDirectoryItem? SelectedItem {
            get => selectedItem;
            set {
                SetProperty(ref selectedItem, value);
                UpdateContent();
            }
        }

        private IFileContent? selectedContent;
        public IFileContent? SelectedContent { get => selectedContent; private set => SetProperty(ref selectedContent, value); }

        public ICommand NavigateCmd { get; }

        public ObjDbBranch(Branch branch) {
            repoRootTree = branch.Tip.Tree;
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = OpenFolder(repoRootTree);
            NavigateCmd = new Command(Navigate);
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == ItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder) {
                var treeToOpen = ((ObjDbDirectoryItem)SelectedItem).GitObject.Peel<Tree>();
                CurrentDirectory = OpenFolder(treeToOpen);
            }
        }

        private ObjDbDirectoryItem[] OpenFolder(Tree folder) => folder
            .OrderBy(x => x, Comparer<TreeEntry>.Create(DirectoriesFirst))
            .Select(x => new ObjDbDirectoryItem(x))
            .ToArray();

        private int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _             ) => -1,
            (_             , Mode.Directory) => 1,
            (_             , _             ) => a.Name.CompareTo(b.Name)
        };
    }
}
