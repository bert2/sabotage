namespace hyperactive {
    using System.IO;

    public class WTreeFileContent: ViewModel, IFileContent {
        public readonly string fullPath;

        public bool ReadOnly { get; }

        private string? content;
        public string? Content {
            get => content;
            set {
                if (SetProperty(ref content, value)) {
                    File.WriteAllText(fullPath, value);
                    Events.RaiseWorkingTreeChanged();
                }
            }
        }

        public WTreeFileContent(string fullPath) {
            this.fullPath = fullPath;
            Content = File.ReadAllText(fullPath);
        }
    }
}
