namespace hyperactive {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class Main : ViewModel, IDataErrorInfo {
        private string? directory = @"D:\DEV\git-conflicts"; // TODO: remove test value
        public string? Directory {
            get => directory;
            set { if (SetProp(ref directory, value)) Touched = true; }
        }

        private Repo? repo;
        public Repo? Repo { get => repo; set => SetProp(ref repo, value); }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public ICommand SelectDirectoryCmd => new Command(SelectDirectory);

        public ICommand LoadRepositoryCmd => new Command(LoadRepository);

        public string Error { get; } = "";
        public string this[string columnName] {
            get => columnName switch {
                _ when !Touched => "",
                nameof(Directory) when !IsRepo(Directory) => "not a git repository",
                _ => ""
            };
        }

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
