namespace hyperactive {
    public class EnterCommitMessage : ViewModel {
        private string? commitMessage;
        public string? CommitMessage {
            get => commitMessage;
            set {
                if (SetProperty(ref commitMessage, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProperty(ref touched, value); }
    }
}
