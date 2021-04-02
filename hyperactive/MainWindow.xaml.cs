namespace hyperactive {
    using System.Windows;
    using System.Windows.Controls;

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new Repo();
        }

        private void FileEditorLeft(object sender, RoutedEventArgs e) {
            var editor = (TextBox)sender;
            editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
