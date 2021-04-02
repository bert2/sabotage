namespace hyperactive {
    using LibGit2Sharp;

    public class ObjDbFileContent: ViewModel, IFileContent {
        public bool ReadOnly { get; } = true;

        private readonly string? content;
        public string? Content { get => content; set { /*noop*/ } }

        public ObjDbFileContent(Blob blob) => content = blob.GetContentText();
    }
}
