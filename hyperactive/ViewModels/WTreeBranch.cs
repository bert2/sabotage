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

        private string currentPath;

        private string name = null!;
        public string Name { get => name; private set => SetProperty(ref name, value); }

        private bool isHead;
        public bool IsHead { get => isHead; private set => SetProperty(ref isHead, value); }

        private IDirectoryItem[] currentDirectory = null!;
        public IDirectoryItem[] CurrentDirectory { get => currentDirectory; private set => SetProperty(ref currentDirectory, value); }

        private IDirectoryItem? selectedItem;
        public IDirectoryItem? SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public ICommand NavigateCmd => new Command(Navigate);

        public ICommand CreateFolderCmd => new Command(CreateFolder);

        public ICommand CreateFileCmd => new Command(CreateFile);

        public ICommand RenameItemCmd => new Command(RenameItem);

        public ICommand DeleteItemCmd => new Command(DeleteItem);

        public WTreeBranch(string repoDirectory, Branch branch) {
            repoRootPath = Path.TrimEndingDirectorySeparator(repoDirectory);
            Name = branch.FriendlyName;
            IsHead = branch.IsCurrentRepositoryHead;
            OpenFolder(repoRootPath);
        }

        private void Navigate() {
            Debug.Assert(SelectedItem is not null);

            if (SelectedItem.Type == ItemType.Folder)
                OpenFolder(((WTreeDirectoryItem)SelectedItem).Path);
            else if (SelectedItem.Type == ItemType.File)
                RenameItem();
        }

        private void OpenFolder(string path) {
            CurrentDirectory = LoadFolder(new DirectoryInfo(path));
            currentPath = repoRootPath;
        }

        private void ReloadCurrentFolder() => CurrentDirectory = LoadFolder(new DirectoryInfo(currentPath));

        private WTreeDirectoryItem[] LoadFolder(DirectoryInfo folder) => folder
            .EnumerateFileSystemInfos()
            .Where(item => item.Name != ".git")
            .OrderBy(item => item, Comparer<FileSystemInfo>.Create(DirectoriesFirst))
            .Select(item => new WTreeDirectoryItem(item))
            .Insert(
                folder.FullName.IsSubPathOf(repoRootPath)
                    ? new[] { new WTreeDirectoryItem("[ .. ]", folder.Parent!.FullName) }
                    : Enumerable.Empty<WTreeDirectoryItem>(),
                index: 0)
            .ToArray();

        private async void CreateFolder() {
            var (ok, folderName) = await Dialog.Show(new EnterNewItemName(ItemType.Folder), vm => vm.Name);
            if (!ok) return;

            Debug.Assert(folderName is not null);

            Directory.CreateDirectory(Path.Combine(currentPath, folderName));

            Snackbar.Show("folder created");

            ReloadCurrentFolder();
        }

        private async void CreateFile() {
            var (ok, fileName) = await Dialog.Show(new EnterNewItemName(ItemType.File), vm => vm.Name);
            if (!ok) return;

            Debug.Assert(fileName is not null);

            File.Open(Path.Combine(currentPath, fileName), FileMode.CreateNew)
                .Dispose();

            Snackbar.Show("file created");

            ReloadCurrentFolder();
        }

        private async void RenameItem() {
            Debug.Assert(SelectedItem is not null);

            var type = SelectedItem.Type.ToString().ToLower();

            var (ok, target) = await Dialog.Show(new EnterNewItemName(ItemType.Folder), vm => vm.Name);
            if (!ok) return;

            // TODO: implement

            Snackbar.Show($"{type} renamed");

            ReloadCurrentFolder();
        }

        private async void DeleteItem() {
            Debug.Assert(SelectedItem is not null);

            var type = SelectedItem.Type.ToString().ToLower();

            if (!await Dialog.Show(new Confirm($"delete {type}", SelectedItem.Name)))
                return;

            // TODO: implement

            Snackbar.Show($"{type} deleted");

            ReloadCurrentFolder();
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

        public override string ToString() => Name;
    }
}
