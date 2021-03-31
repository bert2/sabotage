namespace hyperactive {
    using System;
    using System.IO;

    public class WTreeDirectoryItem: IDirectoryItem {
        public string Name { get; }

        public string Path { get; }

        public ItemType Type { get; }

        public WTreeDirectoryItem(FileSystemInfo fsi)
            : this(fsi.Name, fsi.FullName, GetItemType(fsi)) { }

        public WTreeDirectoryItem(string name, string path, ItemType type)
            => (Name, Path, Type) = (name, path, type);

        public IFileContent ToFileContent() => Type == hyperactive.ItemType.File
            ? new WTreeFileContent(Path)
            : throw new InvalidOperationException($"Cannot get content of {Type}.");

        private static ItemType GetItemType(FileSystemInfo fsi)
            => (fsi.Attributes & FileAttributes.Directory) != 0
                ? ItemType.Folder
                : ItemType.File;
    }
}
