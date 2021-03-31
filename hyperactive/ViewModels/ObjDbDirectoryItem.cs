namespace hyperactive {
    using System;

    using LibGit2Sharp;

    public class ObjDbDirectoryItem: IDirectoryItem {
        public string Name { get; }

        public GitObject GitObject { get; }

        public ItemType Type { get; }

        public ObjDbDirectoryItem Parent { get; }

        public ObjDbDirectoryItem(TreeEntry entry, ObjDbDirectoryItem parent)
            : this(entry.Name, entry.Target, GetItemType(entry), parent) { }

        public ObjDbDirectoryItem(string name, GitObject gitObject, ItemType type, ObjDbDirectoryItem parent)
            => (Name, GitObject, Type, Parent) = (name, gitObject, type, parent);

        public IFileContent ToFileContent() => Type == ItemType.File
            ? new ObjDbFileContent(GitObject.Peel<Blob>())
            : throw new InvalidOperationException($"Cannot get content of {Type}.");

        private static ItemType GetItemType(TreeEntry entry) => entry.TargetType switch {
            TreeEntryTargetType.Tree => ItemType.Folder,
            TreeEntryTargetType.Blob => ItemType.File,
            _ => throw new InvalidOperationException($"{entry.TargetType}s are not supported.")
        };
    }
}
