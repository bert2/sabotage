namespace hyperactive {
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public class Main : ValidatableViewModel {
        private string? directory;
        public string? Directory { get => directory; set => SetProp(ref directory, value); }

        private Repo? repo;
        public Repo? Repo { get => repo; set => SetProp(ref repo, value); }

        public ICommand SelectDirectoryCmd => new Command(SelectDirectory);

        public ICommand LoadRepositoryCmd => new Command(LoadRepository);

        protected override string? Validate(string property) => property switch {
            nameof(Directory) when !System.IO.Directory.Exists(Directory) => "not found",
            nameof(Directory) when !IsRepo(Directory) => "not a git repository",
            _ => null
        };

        private async void SelectDirectory() {
            using var dialog = new FolderBrowserDialog { Description = "Select Git Repository", UseDescriptionForTitle = true };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var path = dialog.SelectedPath;

            if (!Repository.IsValid(path)) {
                if (!await Dialog.Show(new Confirm("do you want to init a git repository in", subject: path)))
                    return;

                Repository.Init(path);

                // Init() creates a symlink like "_git2_a05400 -> testing" for some reason
                System.IO.Directory
                    .EnumerateFileSystemEntries(dialog.SelectedPath, "_git2_*")
                    .ForEach(System.IO.Directory.Delete);
            }

            Directory = path;
            LoadRepository();
        }

        private void LoadRepository() {
            Repo?.Dispose();
            Repo = null; // ensure bound controls don't hang onto old repo in case of load errors
            Repo = new Repo(Directory.NotNull());
        }

        private static bool IsRepo(string? path) => !string.IsNullOrWhiteSpace(path) && Repository.IsValid(path);
    }
}
