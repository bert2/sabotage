namespace hyperactive {
    using LibGit2Sharp;

    using MoreLinq;

    public class StatusViewModel : ViewModel {
        private int wdirAdd;
        public int WdirAdd { get => wdirAdd; private set => SetProperty(ref wdirAdd, value); }

        private int wdirMod;
        public int WdirMod { get => wdirMod; private set => SetProperty(ref wdirMod, value); }

        private int wdirDel;
        public int WdirDel { get => wdirDel; private set => SetProperty(ref wdirDel, value); }

        private int wdirCon;
        public int WdirCon { get => wdirCon; private set => SetProperty(ref wdirCon, value); }

        private int idxAdd;
        public int IdxAdd { get => idxAdd; private set => SetProperty(ref idxAdd, value); }

        private int idxMod;
        public int IdxMod { get => idxMod; private set => SetProperty(ref idxMod, value); }

        private int idxDel;
        public int IdxDel { get => idxDel; private set => SetProperty(ref idxDel, value); }

        private int idxCon;
        public int IdxCon { get => idxCon; private set => SetProperty(ref idxCon, value); }

        public StatusViewModel() { }

        public StatusViewModel(RepositoryStatus status) => status.ForEach(x => {
            switch (x.State) {
                case FileStatus.NewInWorkdir:       wdirAdd++; break;
                case FileStatus.ModifiedInWorkdir:  wdirMod++; break;
                case FileStatus.DeletedFromWorkdir: wdirDel++; break;
                case FileStatus.Conflicted:         wdirCon++; break;

                case FileStatus.NewInIndex:         idxAdd++; break;
                case FileStatus.ModifiedInIndex:    idxMod++; break;
                case FileStatus.DeletedFromIndex:   idxDel++; break;

                case FileStatus.Ignored:
                case FileStatus.Nonexistent:
                case FileStatus.RenamedInWorkdir:
                case FileStatus.RenamedInIndex:
                case FileStatus.TypeChangeInWorkdir:
                case FileStatus.TypeChangeInIndex:
                case FileStatus.Unaltered:
                case FileStatus.Unreadable:
                    break;
            }
        });
    }
}
