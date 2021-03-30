namespace hyperactive {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        public ICommand FooCmd { get; }

        public WTreeBranch(string repoDirectory, Branch branch) {
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            CurrentDirectory = new DirectoryInfo(repoDirectory)
                .EnumerateFileSystemInfos()
                .Where(x => x.Name != ".git")
                .OrderBy(x => x, Comparer<FileSystemInfo>.Create(DirectoriesFirst))
                .Select(x => new WTreeDirectoryItem(x))
                .ToArray();
            FooCmd = new Command(() => { });
        }

        private void UpdateContent() => SelectedContent = SelectedItem?.Type == DirectoryItemType.File
            ? SelectedItem.ToFileContent()
            : null;

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
