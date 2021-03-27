namespace hyperactive {
    using LibGit2Sharp;

    public class FileViewModel: ViewModel {
        public string Name { get; }
        public string Path { get; }
        public bool IsDirty { get; private set; }

        private string? content;
        public string? Content {
            get => content;
            set => IsDirty = SetProperty(ref content, value);
        }

        public FileViewModel(string name, string path, Blob blob) {
            Name = name;
            Path = path;
            content = blob.GetContentText();
        }
    }
}
