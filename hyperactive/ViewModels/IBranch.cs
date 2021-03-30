namespace hyperactive {
    using System.Windows.Input;

    public interface IBranch {
        public string Name { get; }
        public bool IsHead { get; }
        public IDirectoryItem[] CurrentDirectory { get; }
        public IDirectoryItem? SelectedItem { get; set; }
        public IFileContent? SelectedContent { get; }
        public ICommand NavigateCmd { get; }
    }
}
