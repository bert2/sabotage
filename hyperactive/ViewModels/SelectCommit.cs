namespace hyperactive {
    using System.Collections.Generic;

    public class SelectCommit : ViewModel {
        public IBranch MergeTarget { get; }

        private IEnumerable<IBranch> commits;
        public IEnumerable<IBranch> Commits { get => commits; private set => SetProperty(ref commits, value); }

        private IBranch? selectedCommit;
        public IBranch? SelectedCommit { get => selectedCommit; set => SetProperty(ref selectedCommit, value); }

        public SelectCommit(IBranch target, IEnumerable<IBranch> commits)
            => (MergeTarget, this.commits) = (target, commits);
    }
}
