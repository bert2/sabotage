namespace hyperactive {
    using System;
    using System.IO;

    public class WTreeDirectoryItem: IDirectoryItem {
        private readonly FileSystemInfo fsi;

        public string Name => fsi.Name;

        public string Path => fsi.FullName;

        public DirectoryItemType Type => (fsi.Attributes & FileAttributes.Directory) != 0
            ? DirectoryItemType.Folder
            : DirectoryItemType.File;

        public WTreeDirectoryItem(FileSystemInfo fsi) => this.fsi = fsi;

        public IFileContent ToFileContent() => Type == DirectoryItemType.File
            ? new WTreeFileContent(Path)
            : throw new InvalidOperationException($"Cannot get content of {Type}.");
    }
}
