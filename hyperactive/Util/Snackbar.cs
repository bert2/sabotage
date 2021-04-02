namespace hyperactive {
    using System;

    using MaterialDesignThemes.Wpf;

    public static class Snackbar {
        public static readonly ISnackbarMessageQueue Queue = new SnackbarMessageQueue(messageDuration: TimeSpan.FromSeconds(3));

        public static void Show(string message) => Queue.Enqueue(message);
    }
}
