namespace hyperactive {
    using System.Collections.Generic;

    public class SelectMergeSource : ViewModel {
        public LocalBranch Target { get; }

        private IEnumerable<LocalBranch> sources;
        public IEnumerable<LocalBranch> Sources { get => sources; private set => SetProp(ref sources, value); }

        private LocalBranch? selectedSource;
        public LocalBranch? SelectedSource { get => selectedSource; set => SetProp(ref selectedSource, value); }

        public SelectMergeSource(LocalBranch target, IEnumerable<LocalBranch> sources)
            => (Target, this.sources) = (target, sources);
    }
}
