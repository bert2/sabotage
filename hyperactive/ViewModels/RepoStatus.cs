namespace hyperactive {
    using LibGit2Sharp;

    using MoreLinq;

    public class RepoStatus {
        public string? Head { get; }

        public bool IsClean { get; }

        public int WdirAdd { get; private set; }

        public int WdirMod { get; private set; }

        public int WdirDel { get; private set; }

        public int WdirCon { get; private set; }

        public int IdxAdd { get; private set; }

        public int IdxMod { get; private set; }

        public int IdxDel { get; private set; }

        public RepoStatus() { }

        public RepoStatus(Repository repo) {
            Head = repo.Head.FriendlyName;
            var statusEntries = repo.RetrieveStatus();
            IsClean = !statusEntries.IsDirty;
            statusEntries.ForEach(x => {
                switch (x.State) {
                    case FileStatus.NewInWorkdir:        WdirAdd++; break;

                    case FileStatus.ModifiedInWorkdir:
                    case FileStatus.RenamedInWorkdir:
                    case FileStatus.TypeChangeInWorkdir: WdirMod++; break;

                    case FileStatus.DeletedFromWorkdir:  WdirDel++; break;

                    case FileStatus.Conflicted:          WdirCon++; break;

                    case FileStatus.NewInIndex:          IdxAdd++; break;

                    case FileStatus.ModifiedInIndex:
                    case FileStatus.RenamedInIndex:
                    case FileStatus.TypeChangeInIndex:   IdxMod++; break;

                    case FileStatus.DeletedFromIndex:    IdxDel++; break;

                    case FileStatus.Ignored:
                    case FileStatus.Nonexistent:
                    case FileStatus.Unaltered:
                    case FileStatus.Unreadable:
                        break;
                }
            });
        }
    }
}
