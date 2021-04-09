namespace hyperactive {
    using System;

    using MaterialDesignThemes.Wpf;

    public static class Snackbar {
        public static readonly ISnackbarMessageQueue Queue = new SnackbarMessageQueue(messageDuration: TimeSpan.FromSeconds(2));

        public static void Show(string message) => Queue.Enqueue(message);

        public static void ShowImportant(string message) => Queue.Enqueue(
            message,
            actionContent: null,
            actionHandler: null,
            actionArgument: null,
            promote: true,
            neverConsiderToBeDuplicate: true,
            durationOverride: TimeSpan.FromSeconds(5));
    }
}
