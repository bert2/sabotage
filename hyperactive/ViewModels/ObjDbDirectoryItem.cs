namespace hyperactive {
    using System;

    using LibGit2Sharp;

    public class ObjDbDirectoryItem: IDirectoryItem {
        private readonly TreeEntry treeEntry;

        public string Name => treeEntry.Name;

        public string Path => treeEntry.Path;

        public ItemType Type => treeEntry.Mode == Mode.Directory
            ? ItemType.Folder
            : ItemType.File;

        public ObjDbDirectoryItem(TreeEntry treeEntry) => this.treeEntry = treeEntry;

        public IFileContent ToFileContent() => Type == ItemType.File
            ? new ObjDbFileContent(treeEntry.Target.Peel<Blob>())
            : throw new InvalidOperationException($"Cannot get content of {Type}.");
    }
}
