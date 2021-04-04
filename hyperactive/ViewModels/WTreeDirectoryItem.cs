namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using LibGit2Sharp;

    using MoreLinq;

    public class WTreeDirectoryItem: IDirectoryItem {
        public string Name { get; }

        public string Path { get; }

        public ItemType Type { get; }

        public ItemStatus Status { get; }

        public WTreeDirectoryItem(FileSystemInfo fsi)
            : this(fsi.Name, fsi.FullName, GetItemType(fsi), GetItemStatus(fsi)) { }

        public WTreeDirectoryItem(string name, string path, ItemType type)
            : this(name, path, type, ItemStatus.Unchanged) { }

        public WTreeDirectoryItem(string name, string path, ItemType type, ItemStatus status)
            => (Name, Path, Type, Status) = (name, path, type, status);

        public IFileContent ToFileContent() => Type == ItemType.File
            ? new WTreeFileContent(Path)
            : throw new InvalidOperationException($"Cannot get content of {Type}.");

        private static ItemType GetItemType(FileSystemInfo fsi)
            => (fsi.Attributes & FileAttributes.Directory) != 0
                ? ItemType.Folder
                : ItemType.File;

        private static ItemStatus GetItemStatus(FileSystemInfo fsi)
            => GetItemType(fsi) == ItemType.Folder
                ? GetFolderStatus(fsi)
                : GetFileStatus(fsi);

        private static ItemStatus GetFileStatus(FileSystemInfo fsi)
            => GetItemStatus(Repo.Current.NotNull().RetrieveStatus(fsi.FullName));

        private static ItemStatus GetFolderStatus(FileSystemInfo fsi) => Repo.Current
            .NotNull()
            .RetrieveStatus(new StatusOptions {
                PathSpec = new[] {
                    System.IO.Path.GetRelativePath(Repo.CurrentDirectory.NotNull(), fsi.FullName)
                },
                IncludeIgnored = true,
                IncludeUnaltered = true,
                IncludeUntracked = true,
                DetectRenamesInWorkDir = true,
                DetectRenamesInIndex = true,
                RecurseUntrackedDirs = true
            })
            .Select(GetItemStatus)
            .Select(st => st switch {
                ItemStatus.Added   => ItemStatus.Modified,
                ItemStatus.Ignored => ItemStatus.Unchanged,
                _                  => st
            })
            .MaxBy(st => st, Comparer<ItemStatus>.Create(ConflictedFirstUnchangedLast))
            .DefaultIfEmpty(ItemStatus.Unchanged)
            .First();

        private static int ConflictedFirstUnchangedLast(ItemStatus a, ItemStatus b) => (a, b) switch {
            (ItemStatus.Conflicted, _) => 1,
            (_, ItemStatus.Conflicted) => -1,
            (ItemStatus.Unchanged, _) => -1,
            (_, ItemStatus.Unchanged) => 1,
            (_, _) => a.CompareTo(b)
        };

        private static ItemStatus GetItemStatus(StatusEntry status) => GetItemStatus(status.State);

        private static ItemStatus GetItemStatus(FileStatus status) => status switch {
            FileStatus.NewInWorkdir => ItemStatus.Added,
            FileStatus.NewInIndex => ItemStatus.Added,

            FileStatus.ModifiedInWorkdir => ItemStatus.Modified,
            FileStatus.ModifiedInIndex => ItemStatus.Modified,
            FileStatus.RenamedInWorkdir => ItemStatus.Modified,
            FileStatus.RenamedInIndex => ItemStatus.Modified,
            FileStatus.TypeChangeInWorkdir => ItemStatus.Modified,
            FileStatus.TypeChangeInIndex => ItemStatus.Modified,

            FileStatus.Conflicted => ItemStatus.Conflicted,

            FileStatus.Ignored => ItemStatus.Ignored,

            FileStatus.Unaltered => ItemStatus.Unchanged,

            _ => ItemStatus.Unchanged,
        };
    }
}
