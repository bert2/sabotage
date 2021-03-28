namespace hyperactive {
    using System;

    using LibGit2Sharp;

    public class ObjDbFileContent: ViewModel, IFileContent {
        public bool ReadOnly { get; } = true;

        private readonly string? content;
        public string? Content { get => content; set => throw new InvalidOperationException("Cannot edit git objects."); }

        public ObjDbFileContent(Blob blob) => content = blob.GetContentText();
    }
}
