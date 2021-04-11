namespace hyperactive {
    using System;

    public sealed class Events {
        public static Events Instance { get; } = new Events();

        public event EventHandler<EventArgs>? WTreeChanged;

        public event EventHandler<EventArgs>? WTreeAndBranchesChanged;

        private Events() { }

        public static void RaiseWTreeChanged(object? sender = null)
            => Instance.WTreeChanged?.Invoke(sender ?? Instance, EventArgs.Empty);

        public static void RaiseBranchesChanged(object? sender = null)
            => Instance.WTreeAndBranchesChanged?.Invoke(sender ?? Instance, EventArgs.Empty);
    }
}
