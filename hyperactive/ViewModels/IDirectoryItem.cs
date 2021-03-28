namespace hyperactive {
    public enum DirectoryItemType { Folder, File }

    public interface IDirectoryItem {
        public string Name { get; }

        public string Path { get; }

        public DirectoryItemType Type { get; }

        IFileContent ToFileContent();
    }
}
