namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public class ObjDbBranch : ViewModel, IBranch {
        private readonly Tree repoRoot;

        public string Name { get; }

        public bool IsHead { get; }

        private IDirectoryItem[] currentDirectory = null!;
        public IDirectoryItem[] CurrentDirectory { get => currentDirectory; private set => SetProperty(ref currentDirectory, value); }

        private IDirectoryItem? selectedItem;
        public IDirectoryItem? SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public ICommand NavigateCmd => new Command(Navigate);

        public ICommand CreateFolderCmd => new Command(() => throw new NotSupportedException());

        public ICommand CreateFileCmd => new Command(() => throw new NotSupportedException());

        public ICommand RenameItemCmd => new Command(() => throw new NotSupportedException());

        public ICommand DeleteItemCmd => new Command(() => throw new NotSupportedException());

        public ObjDbBranch(Branch branch) {
            repoRoot = branch.Tip.Tree;
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = OpenRootFolder(repoRoot);
        }

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder)
                CurrentDirectory = OpenFolder((ObjDbDirectoryItem)SelectedItem);
        }

        private static ObjDbDirectoryItem[] OpenRootFolder(Tree root)
            => OpenFolder(new ObjDbDirectoryItem(name: "(root)", gitObject: root, parent: null));

        private static ObjDbDirectoryItem[] OpenFolder(ObjDbDirectoryItem folder) => folder
            .GitObject
            .Peel<Tree>()
            .OrderBy(item => item, Comparer<TreeEntry>.Create(DirectoriesFirst))
            .Select(item => new ObjDbDirectoryItem(item, parent: folder))
            .Insert(
                folder.IsRoot
                    ? Enumerable.Empty<ObjDbDirectoryItem>()
                    : new[] { new ObjDbDirectoryItem("[ .. ]", folder.Parent.GitObject, folder.Parent.Parent) },
                index: 0)
            .ToArray();

        private static int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _             ) => -1,
            (_             , Mode.Directory) => 1,
            (_             , _             ) => a.Name.CompareTo(b.Name)
        };

        public override string ToString() => Name;
    }
}
