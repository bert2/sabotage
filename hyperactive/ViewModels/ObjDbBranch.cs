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

        public Repo Parent { get; }

        public string Name { get; }

        public bool IsHead { get; }

        private IDirectoryItem[] currentDirectory = null!;
        public IDirectoryItem[] CurrentDirectory { get => currentDirectory; private set => SetProp(ref currentDirectory, value); }

        private IDirectoryItem? selectedItem;
        public IDirectoryItem? SelectedItem { get => selectedItem; set => SetProp(ref selectedItem, value); }

        public ICommand NavigateCmd => new Command(Navigate);

        public ICommand CreateFolderCmd => new Command(() => throw new NotSupportedException());

        public ICommand CreateFileCmd => new Command(() => throw new NotSupportedException());

        public ICommand RenameItemCmd => new Command(() => throw new NotSupportedException());

        public ICommand DeleteItemCmd => new Command(() => throw new NotSupportedException());

        public ObjDbBranch(Repo parent, Branch branch) {
            Parent = parent;
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

        private ObjDbDirectoryItem[] OpenRootFolder(Tree root)
            => OpenFolder(new ObjDbDirectoryItem(this, name: "(root)", gitObject: root, parentItem: null));

        private ObjDbDirectoryItem[] OpenFolder(ObjDbDirectoryItem folder) => folder
            .GitObject
            .Peel<Tree>()
            .Where(item => item.TargetType is TreeEntryTargetType.Blob or TreeEntryTargetType.Tree)
            .OrderBy(item => item, Comparer<TreeEntry>.Create(DirectoriesFirst))
            .Select(item => new ObjDbDirectoryItem(this, item, parentItem: folder))
            .Insert(
                folder.IsRoot
                    ? Enumerable.Empty<ObjDbDirectoryItem>()
                    : new[] { new ObjDbDirectoryItem(this, "[ .. ]", folder.ParentItem.GitObject, folder.ParentItem.ParentItem) },
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
