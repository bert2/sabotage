namespace hyperactive {
    public class EnterNewBranchName : ViewModel {
        private string? branchName;
        public string? BranchName {
            get => branchName;
            set {
                if (SetProperty(ref branchName, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProperty(ref touched, value); }
    }
}
