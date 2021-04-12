namespace hyperactive {
    using System;

    public sealed class Events {
        public static Events Instance { get; } = new Events();

        public event EventHandler<EventArgs>? WTreeChanged;
        
        public event EventHandler<EventArgs>? WTreeCleared;

        public event EventHandler<EventArgs>? BranchesChanged;

        private Events() { }

        public static void RaiseWTreeChanged(object? sender = null)
            => Instance.WTreeChanged?.Invoke(sender ?? Instance, EventArgs.Empty);

        public static void RaiseWTreeCleared(object? sender = null)
            => Instance.WTreeCleared?.Invoke(sender ?? Instance, EventArgs.Empty);

        public static void RaiseBranchesChanged(object? sender = null)
            => Instance.BranchesChanged?.Invoke(sender ?? Instance, EventArgs.Empty);
    }
}
