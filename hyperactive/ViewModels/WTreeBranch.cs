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
            CurrentDirectory = OpenFolder(new DirectoryInfo(repoDirectory));
            NavigateCmd = new Command(Navigate);
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == ItemType.File
            ? SelectedItem.ToFileContent()
            : null;

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder)
                CurrentDirectory = OpenFolder(new DirectoryInfo(SelectedItem.Path));
            else if (SelectedItem.Type == ItemType.File)
                RenameFile();
        }

        private WTreeDirectoryItem[] OpenFolder(DirectoryInfo folder) => folder
            .EnumerateFileSystemInfos()
            .Where(x => x.Name != ".git")
            .OrderBy(x => x, Comparer<FileSystemInfo>.Create(DirectoriesFirst))
            .Select(x => new WTreeDirectoryItem(x))
            .Prepend(new WTreeDirectoryItem("[ .. ]", (folder.Parent ?? folder).FullName, ItemType.Folder))
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
