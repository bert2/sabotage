namespace hyperactive {
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public class ObjDbBranch : LocalBranch {
        private readonly Tree repoRoot;

        public override ICommand CommitCmd => new InvalidCommand();

        public override ICommand DeleteCmd => new Command(Delete);

        public override ICommand NavigateCmd => new Command(Navigate);

        public override ICommand CreateFolderCmd => new InvalidCommand();

        public override ICommand CreateFileCmd => new InvalidCommand();

        public override ICommand RenameItemCmd => new InvalidCommand();

        public override ICommand DeleteItemCmd => new InvalidCommand();

        public ObjDbBranch(Repo parent, Branch branch) : base(parent, branch) {
            repoRoot = branch.Tip.Tree;
            CurrentDirectory = OpenRootFolder(repoRoot);
        }

        private ObjDbItem[] OpenRootFolder(Tree root)
            => OpenFolder(new ObjDbItem(this, name: "(root)", gitObject: root, parentItem: null));

        private ObjDbItem[] OpenFolder(ObjDbItem folder) => folder
            .GitObject
            .Peel<Tree>()
            .Where(item => item.TargetType is TreeEntryTargetType.Blob or TreeEntryTargetType.Tree)
            .OrderBy(item => item, Comparer<TreeEntry>.Create(DirectoriesFirst))
            .Select(item => new ObjDbItem(this, item, parentItem: folder))
            .Insert(
                folder.IsRoot
                    ? Enumerable.Empty<ObjDbItem>()
                    : new[] { new ObjDbItem(this, "[ .. ]", folder.ParentItem.GitObject, folder.ParentItem.ParentItem) },
                index: 0)
            .ToArray();

        private async void Delete() {
            if (!await Dialog.Show(new Confirm(action: "delete branch", subject: Name)))
                return;

            repo.Branches.Remove(Name);

            Snackbar.Show("branch deleted");

            Events.RaiseBranchesChanged();
        }

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder)
                CurrentDirectory = OpenFolder((ObjDbItem)SelectedItem);
        }

        private static int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _             ) => -1,
            (_             , Mode.Directory) => 1,
            (_             , _             ) => a.Name.CompareTo(b.Name)
        };
    }
}
