namespace hyperactive {
    public class EnterCommitMessage : ValidatableViewModel {
        private string? commitMessage;
        public string? CommitMessage { get => commitMessage; set => SetProp(ref commitMessage, value); }

        protected override string? Validate(string property) => property switch {
            nameof(CommitMessage) when string.IsNullOrWhiteSpace(CommitMessage) => "cannot be empty",
            _ => null
        };
    }
}
