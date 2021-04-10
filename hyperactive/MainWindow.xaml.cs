namespace hyperactive {
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using MaterialDesignExtensions.Controls;

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "event handlers might start with control name")]
    public partial class MainWindow : MaterialWindow {
        public MainWindow() {
            InitializeComponent();
            DataContext = new Repo();
        }

        private void FileEditorLeft(object sender, RoutedEventArgs e) {
            var editor = (TextBox)sender;
            editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void txtDirectory_UnfocusOnReturn(object sender, System.Windows.Input.KeyEventArgs e) {
            //if (e.Key == System.Windows.Input.Key.Return) {
            //    MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
            //}
        }

        // `TextBoxSuggestions` control will not show suggestions for a text box that never had a value before.
        // This `GotFocus` handler forces firing of `TextChanged` event so `TextBoxSuggestions` will initiate a search.
        private void txtDirectory_SearchOnInitialFocus(object sender, RoutedEventArgs e) {
            Trace.WriteLine($"======== focused with value '{txtDirectory.Text}'");
            if (string.IsNullOrEmpty(txtDirectory.Text)) {
                txtDirectory.Text = " ";
                txtDirectory.Text = "";
            }
            //else {
            //    var text = txtDirectory.Text;
            //    txtDirectory.Text = $"{text} ";
            //    txtDirectory.Text = text;
            //}

            var binding = txtDirectory.GetBindingExpression(TextBox.TextProperty);
            //if (binding.Status == System.Windows.Data.BindingStatus.Active)
                binding.UpdateSource();
        }
    }
}
