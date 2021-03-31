namespace hyperactive {
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public class WTreeBranch : ViewModel, IBranch {
        private readonly string repoRootPath;

        private string name;
        public string Name { get => name; private set => SetProperty(ref name, value); }

        private bool isHead;
        public bool IsHead { get => isHead; private set => SetProperty(ref isHead, value); }

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

        public ICommand NavigateCmd => new Command(Navigate);

        public WTreeBranch(string repoDirectory, Branch branch) {
            repoRootPath = Path.TrimEndingDirectorySeparator(repoDirectory);
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = OpenFolder(new DirectoryInfo(repoRootPath));
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == ItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder)
                CurrentDirectory = OpenFolder(new DirectoryInfo(((WTreeDirectoryItem)SelectedItem).Path));
            else if (SelectedItem.Type == ItemType.File)
                RenameFile();
        }

        private WTreeDirectoryItem[] OpenFolder(DirectoryInfo folder) => folder
            .EnumerateFileSystemInfos()
            .Where(item => item.Name != ".git")
            .OrderBy(item => item, Comparer<FileSystemInfo>.Create(DirectoriesFirst))
            .Select(item => new WTreeDirectoryItem(item))
            .Insert(
                folder.FullName.IsSubPathOf(repoRootPath)
                    ? new[] { new WTreeDirectoryItem("[ .. ]", folder.Parent!.FullName, ItemType.Folder) }
                    : Enumerable.Empty<WTreeDirectoryItem>(),
                index: 0)
            .ToArray();

        private static void RenameFile() {
            MessageBox.Show("TODO: rename file dialog");
        }

        private static int DirectoriesFirst(FileSystemInfo a, FileSystemInfo b) {
            var aIsDir = (a.Attributes & FileAttributes.Directory) != 0;
            var bIsDir = (b.Attributes & FileAttributes.Directory) != 0;

            return (aIsDir, bIsDir) switch {
                (true , true ) => a.Name.CompareTo(b.Name),
                (true , false) => -1,
                (false, true ) => 1,
                (false, false) => a.Name.CompareTo(b.Name)
            };
        }
    }
}
