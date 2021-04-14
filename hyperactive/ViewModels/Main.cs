namespace hyperactive {
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class Main : ValidatableViewModel {
        private string? directory = @"D:\DEV\git-empty"; // TODO: remove test value
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

        public Main() => LoadRepository(); // TODO: remove test code

        private async void SelectDirectory() {
            using var dialog = new FolderBrowserDialog {
                Description = "Select Git Repository",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            if (!Repository.IsValid(dialog.SelectedPath)) {
                await Dialog.Show(new Error("not a git repository", details: dialog.SelectedPath));
                return;
            }

            Directory = dialog.SelectedPath;
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
