namespace hyperactive {
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class Main : ViewModel {
        private string? directory = @"D:\DEV\git-conflicts"; // TODO: remove test value
        public string? Directory {
            get => directory;
            set { if (SetProp(ref directory, value)) LoadRepository(); }
        }

        private Repo? repo;
        public Repo? Repo { get => repo; set => SetProp(ref repo, value); }

        public ICommand SelectDirectoryCmd => new Command(SelectDirectory);

        public ICommand LoadRepositoryCmd => new Command(LoadRepository);

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

            if (Repository.IsValid(dialog.SelectedPath))
                Directory = dialog.SelectedPath;
            else
                await Dialog.Show(new Error("not a git repository", details: dialog.SelectedPath));
        }

        private void LoadRepository() {
            Repo?.Dispose();
            Repo = null; // ensure bound controls don't hang onto old repo in case of load errors
            Repo = new Repo(Directory.NotNull());
        }
    }
}
