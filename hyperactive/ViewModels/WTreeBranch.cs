namespace hyperactive {
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class WTreeBranch : ViewModel, IBranch {
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

        public ICommand NavigateCmd { get; }

        public WTreeBranch(string repoDirectory, Branch branch) {
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = OpenFolder(repoDirectory);
            NavigateCmd = new Command(Navigate);
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == DirectoryItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == DirectoryItemType.Folder)
                CurrentDirectory = OpenFolder(SelectedItem.Path);
            else if (SelectedItem.Type == DirectoryItemType.File)
                RenameFile();
        }

        private WTreeDirectoryItem[] OpenFolder(string path) => new DirectoryInfo(path)
            .EnumerateFileSystemInfos()
            .Where(x => x.Name != ".git")
            .OrderBy(x => x, Comparer<FileSystemInfo>.Create(DirectoriesFirst))
            .Select(x => new WTreeDirectoryItem(x))
            .ToArray();

        private static void RenameFile() {
            MessageBox.Show("TODO: rename file dialog");
        }

        private int DirectoriesFirst(FileSystemInfo a, FileSystemInfo b) {
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
