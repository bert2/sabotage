namespace hyperactive {
    public class EnterCommitMessage : ViewModel {
        private string? commitMessage;
        public string? CommitMessage { get => commitMessage; set => SetProperty(ref commitMessage, value); }
    }
}
