namespace hyperactive {
    using System;

    using LibGit2Sharp;

    public class ObjDbFileContent: ViewModel, IFileContent {
        public string Name { get; }

        public string Path { get; }

        public bool ReadOnly { get; } = true;

        private readonly string? content;
        public string? Content { get => content; set => throw new InvalidOperationException("Cannot edit git objects."); }

        public ObjDbFileContent(string name, string path, Blob blob) {
            Name = name;
            Path = path;
            content = blob.GetContentText();
        }
    }
}
