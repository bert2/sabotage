namespace hyperactive {
    using System.Windows.Input;

    public interface IBranch {
        public string Name { get; }

        public bool IsHead { get; }

        public IDirectoryItem[] CurrentDirectory { get; }

        public IDirectoryItem? SelectedItem { get; set; }

        public ICommand NavigateCmd { get; }

        public ICommand CreateFolderCmd { get; }

        public ICommand CreateFileCmd { get; }

        public ICommand RenameItemCmd { get; }

        public ICommand DeleteItemCmd { get; }
    }
}
