namespace hyperactive {
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LibGit2Sharp;

    public class ObjDbDirectoryItem: IDirectoryItem {
        public string Name { get; }

        public GitObject GitObject { get; }

        public ItemType Type { get; }

        public ItemStatus Status { get; } = ItemStatus.Unchanged;

        private string? content;
        public string? Content {
            get => content ??= Type == ItemType.File ? GitObject.Peel<Blob>().GetContentText() : null;
            set { /*noop*/ }
        }

        public bool ReadOnly { get; } = true;

        public ObjDbDirectoryItem? Parent { get; }

        [MemberNotNullWhen(false, nameof(Parent))]
        public bool IsRoot => Parent is null;

        public ObjDbDirectoryItem(TreeEntry entry, ObjDbDirectoryItem? parent)
            => (Name, GitObject, Type, Parent) = (entry.Name, entry.Target, GetItemType(entry), parent);

        /// <summary>Used to create the "[..]" entry that navigates backwards.</summary>
        public ObjDbDirectoryItem(string name, GitObject gitObject, ObjDbDirectoryItem? parent)
            => (Name, GitObject, Type, Parent) = (name, gitObject, ItemType.Folder, parent);

        private static ItemType GetItemType(TreeEntry entry) => entry.TargetType switch {
            TreeEntryTargetType.Tree => ItemType.Folder,
            TreeEntryTargetType.Blob => ItemType.File,
            _ => throw new InvalidOperationException($"{entry.TargetType}s are not supported.")
        };
    }
}
