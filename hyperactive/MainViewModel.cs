namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class MainViewModel: ViewModelBase {
        private string? directory = @"D:\DEV\git-conflicts";
        public string? Directory {
            get => directory;
            set {
                SetProperty(ref directory, value);
                Load();
            }
        }

        private Repository? repo;
        public Repository? Repo {
            get => repo;
            private set => SetProperty(ref repo, value);
        }

        private IEnumerable<Branch>? branches;
        public IEnumerable<Branch>? Branches {
            get => branches;
            private set => SetProperty(ref branches, value);
        }

        public MainViewModel() {
            SelectDirectory = new Command(Select);
            LoadDirectory = new Command(Load, () => Repository.IsValid(Directory));
            Load();
        }

        public ICommand SelectDirectory { get; }

        public ICommand LoadDirectory { get; }

        private void Select() {
            using var dialog = new FolderBrowserDialog {
                Description = "Select Git Repository",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                Directory = dialog.SelectedPath;
                Load();
            }
        }

        private void Load() {
            Repo = new Repository(Directory);
            Branches = repo?.Branches.Where(b => !b.IsRemote);
        }
    }
}
