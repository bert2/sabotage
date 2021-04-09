namespace hyperactive {
    using System;

    public sealed class Events {
        public static Events Instance { get; } = new Events();

        public event EventHandler<EventArgs>? WorkingTreeChanged;

        private Events() { }

        public static void RaiseWorkingTreeChanged(object? sender = null)
            => Instance.WorkingTreeChanged?.Invoke(sender ?? Instance, EventArgs.Empty);
    }
}
