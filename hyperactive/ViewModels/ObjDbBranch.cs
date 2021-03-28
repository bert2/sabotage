namespace hyperactive {
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class ObjDbBranch : ViewModel, IBranch {
        private string name;
        public string Name { get => name; private set => SetProperty(ref name, value); }

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

        public ObjDbBranch(Branch branch) {
            Name = branch.FriendlyName;
            CurrentDirectory = branch.Tip.Tree
                .OrderBy(x => x, Comparer<TreeEntry>.Create(DirectoriesFirst))
                .Select(x => new ObjDbDirectoryItem(x))
                .ToArray();
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == DirectoryItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _) => -1,
            (_, Mode.Directory) => 1,
            (_, _) => a.Name.CompareTo(b.Name)
        };
    }
}
