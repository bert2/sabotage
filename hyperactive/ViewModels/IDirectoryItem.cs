namespace hyperactive {
    public enum ItemType { Folder, File }

    public enum ItemStatus { Unchanged, Added, Modified, Conflicted, Ignored }

    public interface IDirectoryItem {
        public LocalBranch Parent { get; }

        public string Name { get; }

        public ItemType Type { get; }

        public ItemStatus Status { get; }

        public string? Content { get; set; }

        public bool ReadOnly { get; }
    }
}
