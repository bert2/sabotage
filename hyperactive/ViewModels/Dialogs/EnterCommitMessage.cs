namespace hyperactive {
    using System.ComponentModel;

    public class EnterCommitMessage : ViewModel, IDataErrorInfo {
        private string? commitMessage;
        public string? CommitMessage {
            get => commitMessage;
            set { if (SetProp(ref commitMessage, value)) Touched = true; }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public string Error { get; } = "";
        public string this[string columnName] {
            get => columnName switch {
                _ when !Touched => "",
                nameof(CommitMessage) when string.IsNullOrWhiteSpace(CommitMessage) => "cannot be empty",
                _ => ""
            };
        }
    }
}
