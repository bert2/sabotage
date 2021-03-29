namespace hyperactive {
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException((Exception)e.ExceptionObject);

            TaskScheduler.UnobservedTaskException += (_, e) => {
                HandleException(e.Exception);
                e.SetObserved();
            };

            DispatcherUnhandledException += (_, e) => {
                HandleException(e.Exception);
                e.Handled = true;
            };
        }

        private static void HandleException(Exception ex)
            => MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
