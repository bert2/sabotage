namespace hyperactive {
    using System.Windows;

    public partial class MainWindow : Window {
        private readonly MainViewModel mainViewModel = new();

        public MainWindow() {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}
