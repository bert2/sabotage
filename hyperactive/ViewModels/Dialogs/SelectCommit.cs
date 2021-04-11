namespace hyperactive {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using MaterialDesignExtensions.Model;

    public class SelectCommit : ViewModel, IAutocompleteSource {
        public LocalBranch MergeTarget { get; }

        private IEnumerable<Commit_> commits;
        public IEnumerable<Commit_> Commits { get => commits; private set => SetProp(ref commits, value); }

        private Commit_? selectedCommit;
        public Commit_? SelectedCommit { get => selectedCommit; set => SetProp(ref selectedCommit, value); }

        public SelectCommit(LocalBranch target, IEnumerable<Commit_> commits)
            => (MergeTarget, this.commits) = (target, commits);

        public IEnumerable Search(string searchTerm) => Commits.Where(c => c.Matches(searchTerm));
    }
}
