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
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = OpenFolder(repoRoot, current: new ObjDbDirectoryItem("(root)", repoRoot, ItemType.Folder, null));
            NavigateCmd = new Command(Navigate);
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == ItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder) {
                var treeToOpen = ((ObjDbDirectoryItem)SelectedItem).GitObject.Peel<Tree>();
                var current = ((ObjDbDirectoryItem)SelectedItem);
                CurrentDirectory = OpenFolder(treeToOpen, current);
            }
        }

        private ObjDbDirectoryItem[] OpenFolder(Tree folder, ObjDbDirectoryItem current) => folder
            .OrderBy(x => x, Comparer<TreeEntry>.Create(DirectoriesFirst))
            .Select(x => new ObjDbDirectoryItem(x, current))
            .Insert(
                folder != repoRoot
                    ? new[] { new ObjDbDirectoryItem("[ .. ]", current.Parent.GitObject, ItemType.Folder, current.Parent.Parent) }
                    : Enumerable.Empty<ObjDbDirectoryItem>(),
                index: 0)
            .ToArray();

        private int DirectoriesFirst(TreeEntry a, TreeEntry b) => (a.Mode, b.Mode) switch {
            (Mode.Directory, Mode.Directory) => a.Name.CompareTo(b.Name),
            (Mode.Directory, _             ) => -1,
            (_             , Mode.Directory) => 1,
            (_             , _             ) => a.Name.CompareTo(b.Name)
        };

        //private static Tree FindParent(Tree child, Tree start) {
        //    return start.Any(x => x.Target.Id == child.Id)
        //        ? start
        //        : start
        //            .Where(x => x.TargetType == TreeEntryTargetType.Tree)
        //            .Select(x => FindParent(child, x.Target.Peel<Tree>()))

        //}
    }
}
