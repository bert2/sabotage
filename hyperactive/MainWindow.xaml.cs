namespace hyperactive {
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;

    public partial class MainWindow : Window {
        private readonly MainViewModel mainViewModel = new();

        public MainWindow() {
            InitializeComponent();
            DataContext = mainViewModel;
            mainViewModel.Load();
        }

        private void OpenDirectory(object sender, RoutedEventArgs e) {
            using var dialog = new FolderBrowserDialog {
                Description = "Select Git Repository",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                mainViewModel.Directory = dialog.SelectedPath;
                mainViewModel.Load();
            }
        }

        private void LoadDirectory(object sender, RoutedEventArgs e) => mainViewModel.Load();
    }
}
