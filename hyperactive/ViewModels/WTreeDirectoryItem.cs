﻿namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using LibGit2Sharp;

    using MoreLinq;

    public class WTreeDirectoryItem: ViewModel, IDirectoryItem {
        private readonly bool isVirtual;

        public string Name { get; }

        public string Path { get; }

        public ItemType Type { get; }

        public ItemStatus Status =>
            isVirtual ? ItemStatus.Unchanged
            : Type == ItemType.File ? GetFileStatus(Path)
            : GetFolderStatus(Path);

        private string? content;
        public string? Content {
            get => content;
            set {
                if (SetProperty(ref content, value)) {
                    File.WriteAllText(Path, value);
                    Events.RaiseWorkingTreeChanged();
                }
            }
        }

        public WTreeDirectoryItem(FileSystemInfo fsi)
            => (Name, Path, Type, isVirtual) = (fsi.Name, fsi.FullName, GetItemType(fsi), false);

        /// <summary>Used to create the "[..]" entry that navigates backwards.</summary>
        public WTreeDirectoryItem(string name, string path)
            => (Name, Path, Type, isVirtual) = (name, path, ItemType.Folder, true);

        public IFileContent ToFileContent() => Type == ItemType.File
            ? new WTreeFileContent(Path)
            : throw new InvalidOperationException($"Cannot get content of {Type}.");

        private static ItemType GetItemType(FileSystemInfo fsi)
            => (fsi.Attributes & FileAttributes.Directory) != 0
                ? ItemType.Folder
                : ItemType.File;

        private static ItemStatus GetFileStatus(string path) => Repo.Current.NotNull().RetrieveStatus(path) switch {
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

            FileStatus.DeletedFromWorkdir => ItemStatus.Unchanged,
            FileStatus.DeletedFromIndex => ItemStatus.Unchanged,
            FileStatus.Nonexistent => ItemStatus.Unchanged,
            FileStatus.Unreadable => ItemStatus.Unchanged,
            _ => ItemStatus.Unchanged,
        };

        private static ItemStatus GetFolderStatus(string path) => Repo.Current
            .NotNull()
            .RetrieveStatus(new StatusOptions {
                PathSpec = new[] {
                    System.IO.Path.GetRelativePath(Repo.CurrentDirectory.NotNull(), path)
                },
                IncludeUntracked = true,
                DetectRenamesInWorkDir = false,
                DetectRenamesInIndex = false,
                RecurseUntrackedDirs = true
            })
            .Select(st => st.State switch {
                FileStatus.NewInWorkdir => ItemStatus.Modified,
                FileStatus.NewInIndex => ItemStatus.Modified,
                FileStatus.ModifiedInWorkdir => ItemStatus.Modified,
                FileStatus.ModifiedInIndex => ItemStatus.Modified,
                FileStatus.RenamedInWorkdir => ItemStatus.Modified,
                FileStatus.RenamedInIndex => ItemStatus.Modified,
                FileStatus.TypeChangeInWorkdir => ItemStatus.Modified,
                FileStatus.TypeChangeInIndex => ItemStatus.Modified,
                FileStatus.DeletedFromWorkdir => ItemStatus.Modified,
                FileStatus.DeletedFromIndex => ItemStatus.Modified,

                FileStatus.Conflicted => ItemStatus.Conflicted,

                FileStatus.Unaltered => ItemStatus.Unchanged,
                FileStatus.Ignored => ItemStatus.Unchanged,
                FileStatus.Nonexistent => ItemStatus.Unchanged,
                FileStatus.Unreadable => ItemStatus.Unchanged,
                _ => ItemStatus.Unchanged,
            })
            .MaxBy(st => st, Comparer<ItemStatus>.Create(ConflictedFirstUnchangedLast))
            .DefaultIfEmpty(ItemStatus.Unchanged)
            .First();

        private static int ConflictedFirstUnchangedLast(ItemStatus a, ItemStatus b) => (a, b) switch {
            (ItemStatus.Conflicted, _) => 1,
            (_, ItemStatus.Conflicted) => -1,
            (ItemStatus.Unchanged, _) => -1,
            (_, ItemStatus.Unchanged) => 1,
            (ItemStatus.Modified, ItemStatus.Modified) => 0,
            _ => throw new InvalidOperationException($"Unexpected folder item status when comparing {a} vs {b}.")
        };
    }
}
