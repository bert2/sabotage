namespace hyperactive {
    public enum ItemType { Folder, File }

    public interface IDirectoryItem {
        public string Name { get; }

        public ItemType Type { get; }

        IFileContent ToFileContent();
    }
}
