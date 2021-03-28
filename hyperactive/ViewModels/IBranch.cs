namespace hyperactive {
    public interface IBranch {
        public string Name { get; }
        public IDirectoryItem[] CurrentDirectory { get; }
        public IDirectoryItem? SelectedItem { get; set; }
        public IFileContent? SelectedContent { get; }
    }
}
