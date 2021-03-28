namespace hyperactive {
    using System;

    using LibGit2Sharp;

    public class ObjDbDirectoryItem: IDirectoryItem {
        private readonly TreeEntry treeEntry;

        public string Name => treeEntry.Name;

        public string Path => treeEntry.Path;

        public DirectoryItemType Type => treeEntry.Mode == Mode.Directory
            ? DirectoryItemType.Folder
            : DirectoryItemType.File;

        public ObjDbDirectoryItem(TreeEntry treeEntry) => this.treeEntry = treeEntry;

        public IFileContent ToFileContent() => Type == DirectoryItemType.File
            ? new ObjDbFileContent(treeEntry.Target.Peel<Blob>())
            : throw new InvalidOperationException($"Cannot get content of {Type}.");
    }
}
