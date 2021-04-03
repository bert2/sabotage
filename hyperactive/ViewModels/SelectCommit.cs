namespace hyperactive {
    using System.Collections.Generic;

    public class SelectCommit : ViewModel {
        public IBranch MergeTarget { get; }

        private IEnumerable<Commit_> commits;
        public IEnumerable<Commit_> Commits { get => commits; private set => SetProperty(ref commits, value); }

        private Commit_? selectedCommit;
        public Commit_? SelectedCommit { get => selectedCommit; set => SetProperty(ref selectedCommit, value); }

        public SelectCommit(IBranch target, IEnumerable<Commit_> commits)
            => (MergeTarget, this.commits) = (target, commits);
    }
}
