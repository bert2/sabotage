namespace hyperactive {
    using LibGit2Sharp;

    using MoreLinq;

    public enum WTreeStatus { Clean, Dirty, Conflicted }

    public class RepoStatus {
        public string? Head { get; }

        public CurrentOperation CurrentOperation { get; }

        public WTreeStatus WTreeStatus { get; }

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

            CurrentOperation = repo.Info.CurrentOperation;

            System.Diagnostics.Trace.WriteLine($"##### start status ({Repo.Instance?.Directory})");
            var entries = repo.RetrieveStatus();
            System.Diagnostics.Trace.WriteLine($"##### DONE status ({Repo.Instance?.Directory})");

            entries.ForEach(x => {
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

            WTreeStatus =
                WdirCon > 0     ? WTreeStatus.Conflicted :
                entries.IsDirty ? WTreeStatus.Dirty :
                                  WTreeStatus.Clean;
        }
    }
}
