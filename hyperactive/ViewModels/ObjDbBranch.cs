namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public class ObjDbBranch : LocalBranch {
        private readonly Tree repoRoot;

        public override ICommand NavigateCmd => new Command(Navigate);

        public override ICommand CreateFolderCmd => new Command(() => throw new NotSupportedException());

        public override ICommand CreateFileCmd => new Command(() => throw new NotSupportedException());

        public override ICommand RenameItemCmd => new Command(() => throw new NotSupportedException());

        public override ICommand DeleteItemCmd => new Command(() => throw new NotSupportedException());

        public ObjDbBranch(Repo parent, Branch branch) : base(parent, branch) {
            repoRoot = branch.Tip.Tree;
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
    }
}
