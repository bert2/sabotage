namespace hyperactive {
    using System;

    using MaterialDesignThemes.Wpf;

    public static class Snackbar {
        public static readonly SnackbarMessageQueue Queue = new(messageDuration: TimeSpan.FromSeconds(2));

        public static void Show(string message) => ShowImmediately(message);

        public static void ShowImportant(string message) => ShowImmediately(message, duration: TimeSpan.FromSeconds(5));

        private static void ShowImmediately(string message, TimeSpan? duration = null) {
            Queue.Clear(); // we don't want the queueing behavior
            Queue.Enqueue(
                message,
                actionContent: null,
                actionHandler: null,
                actionArgument: null,
                promote: true,
                neverConsiderToBeDuplicate: true,
                durationOverride: duration);
        }
    }
}
