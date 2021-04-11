namespace hyperactive {
    public class EnterNewBranchName : ViewModel {
        public string? OldName { get; }

        private string? branchName;
        public string? BranchName {
            get => branchName;
            set {
                if (SetProp(ref branchName, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public EnterNewBranchName(string? oldName = null) => OldName = oldName;
    }
}
