namespace hyperactive {
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LibGit2Sharp;

    public class ObjDbItem: IDirectoryItem {
        public LocalBranch Parent { get; }

        public string Name { get; }

        public GitObject GitObject { get; }

        public ItemType Type { get; }

        public ItemStatus Status { get; } = ItemStatus.Unchanged;

        private string? content;
        public string? Content {
            get => content ??= Type == ItemType.File ? GitObject.Peel<Blob>().GetContentText() : null;
            set => throw new NotSupportedException();
        }

        public bool ReadOnly { get; } = true;

        public ObjDbItem? ParentItem { get; }

        [MemberNotNullWhen(false, nameof(ParentItem))]
        public bool IsRoot => ParentItem is null;

        public ObjDbItem(ObjDbBranch parent, TreeEntry entry, ObjDbItem? parentItem)
            => (Parent, Name, GitObject, Type, ParentItem) = (parent, entry.Name, entry.Target, GetItemType(entry), parentItem);

        // used to create the "[..]" entry that navigates backwards
        public ObjDbItem(ObjDbBranch parent, string name, GitObject gitObject, ObjDbItem? parentItem)
            => (Parent, Name, GitObject, Type, ParentItem) = (parent, name, gitObject, ItemType.Folder, parentItem);

        private static ItemType GetItemType(TreeEntry entry) => entry.TargetType switch {
            TreeEntryTargetType.Tree => ItemType.Folder,
            TreeEntryTargetType.Blob => ItemType.File,
            _ => throw new NotSupportedException($"{entry.TargetType}s are not supported.")
        };
    }
}
