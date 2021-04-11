namespace hyperactive {
    using System.Windows.Controls;

    using MaterialDesignExtensions.Controls;

    public partial class MainWindow : MaterialWindow {
        public MainWindow() {
            InitializeComponent();
            DataContext = new Main();
        }

        private void UpdateTextBoxBindingOnReturn(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Return && sender is TextBox textBox) {
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
