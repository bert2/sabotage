namespace hyperactive {
    using System.Collections.Generic;

    public class SelectMergeSource : ViewModel {
        public IBranch Target { get; }

        private IEnumerable<IBranch> sources;
        public IEnumerable<IBranch> Sources { get => sources; private set => SetProperty(ref sources, value); }

        private IBranch? selectedSource;
        public IBranch? SelectedSource { get => selectedSource; set => SetProperty(ref selectedSource, value); }

        public SelectMergeSource(IBranch target, IEnumerable<IBranch> sources)
            => (Target, this.sources) = (target, sources);
    }
}
