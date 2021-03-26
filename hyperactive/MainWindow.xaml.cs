namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;

    using LibGit2Sharp;

    public partial class MainWindow : Window, INotifyPropertyChanged {
        public MainWindow() => InitializeComponent();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private string directory = @"D:\DEV\git-conflicts";
        public string Directory {
            get => directory;
            set {
                if (value != directory) {
                    directory = value;
                    OnPropertyChanged(nameof(Directory));
                }
            }
        }

        private Repository? repo;
        private Repository? Repo {
            get => repo;
            set {
                if (value != repo) {
                    repo = value;
                    OnPropertyChanged(nameof(Repo));
                    OnPropertyChanged(nameof(Branches));
                }
            }
        }

        public IEnumerable<Branch>? Branches => repo?.Branches.Where(b => !b.IsRemote);

        private void OpenDirectory(object sender, RoutedEventArgs e) {
            using var dialog = new FolderBrowserDialog {
                Description = "Select Git Repository",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Directory = dialog.SelectedPath;
                LoadDirectory(sender: null!, e: null!);
            }
        }

        private void LoadDirectory(object sender, RoutedEventArgs e) {
            if (!Repository.IsValid(Directory)) return;
            Repo = new Repository(Directory);
        }
    }
}
