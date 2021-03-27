namespace hyperactive {
    using LibGit2Sharp;

    public class BranchViewModel : ViewModel {
        private string name;
        public string Name { get => name; private set => SetProperty(ref name, value); }

        public BranchViewModel(Branch branch) {
            Name = branch.FriendlyName;
        }
    }
}
