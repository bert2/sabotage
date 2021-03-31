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
        private readonly Stack<Tree> parents = new();

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
            repoRoot = branch.Tip.Tree;
            parents.Push(repoRoot);
            Trace.WriteLine(string.Join(',', parents.Select(x => x.Sha.Substring(0, 6))));
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = OpenFolder(repoRoot, parents);
            NavigateCmd = new Command(Navigate);
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == ItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder) {
                var treeToOpen = ((ObjDbDirectoryItem)SelectedItem).GitObject.Peel<Tree>();

                var back = SelectedItem.Name == "[ .. ]";
                if (back)
                    parents.Pop();

                Trace.WriteLine(string.Join(',', parents.Select(x => x.Sha.Substring(0, 6))));

                CurrentDirectory = OpenFolder(treeToOpen, parents);

                if (!back)
                    parents.Push(treeToOpen);

                Trace.WriteLine(string.Join(',', parents.Select(x => x.Sha.Substring(0, 6))));
            }
        }

        private ObjDbDirectoryItem[] OpenFolder(Tree folder, Stack<Tree> parents) => folder
            .OrderBy(x => x, Comparer<TreeEntry>.Create(DirectoriesFirst))
            .Select(x => new ObjDbDirectoryItem(x))
            .Insert(
                folder != repoRoot
                    ? new[] { new ObjDbDirectoryItem("[ .. ]", parents.Peek(), ItemType.Folder) }
                    : Enumerable.Empty<ObjDbDirectoryItem>(),
                index: 0)
            .ToArray();

        private int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _             ) => -1,
            (_             , Mode.Directory) => 1,
            (_             , _             ) => a.Name.CompareTo(b.Name)
        };
    }
}
