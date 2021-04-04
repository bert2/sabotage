namespace hyperactive {
    public enum ItemType { Folder, File }

    public enum ItemStatus { Unchanged, Added, Modified, Conflicted, Ignored }

    public interface IDirectoryItem {
        public string Name { get; }

        public ItemType Type { get; }

        public ItemStatus Status { get; }

        IFileContent ToFileContent();
    }
}
