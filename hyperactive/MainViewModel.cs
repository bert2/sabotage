﻿namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class MainViewModel: ViewModelBase {
        private string? directory;// = @"D:\DEV\git-conflicts"; // TODO: remove test value
        public string? Directory {
            get => directory;
            set {
                SetProperty(ref directory, value);
                LoadRepository();
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

        private bool isLoaded;
        public bool IsLoaded {
            get => isLoaded;
            private set => SetProperty(ref isLoaded, value);
        }

        public MainViewModel() {
            SelectDirectoryCmd = new Command(SelectDirectory);
            LoadRepositoryCmd = new Command(LoadRepository);
            //LoadRepository(); // TODO: remove test code
        }

        public ICommand SelectDirectoryCmd { get; }

        public ICommand LoadRepositoryCmd { get; }

        private void SelectDirectory() {
            using var dialog = new FolderBrowserDialog {
                Description = "Select Git Repository",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                Directory = dialog.SelectedPath;
            }
        }

        private void LoadRepository() {
            Repo = new Repository(Directory);
            Branches = repo?.Branches.Where(b => !b.IsRemote);
            IsLoaded = true;
        }
    }
}
