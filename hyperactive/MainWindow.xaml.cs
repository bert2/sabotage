namespace hyperactive {
    using System.Windows;
    using System.Windows.Controls;

    using MaterialDesignExtensions.Controls;

    public partial class MainWindow : MaterialWindow {
        public MainWindow() {
            InitializeComponent();
            DataContext = new Main();
        }

        private void LoadRepoOnReturn(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Return
                && DataContext is Main main
                && main[nameof(Main.Directory)]?.Length == 0) {
                main.LoadRepositoryCmd.Execute(parameter: null);
            }
        }

        private void UpdateTextBoxBindingOnLeave(object sender, RoutedEventArgs e) {
            if (sender is TextBox textBox && !textBox.IsReadOnly) {
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
