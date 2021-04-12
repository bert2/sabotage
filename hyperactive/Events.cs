namespace hyperactive {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Events {
        public static Events Instance { get; } = new Events();

        public event EventHandler<EventArgs>? WTreeChanged;

        public event EventHandler<EventArgs>? WTreeCleared;

        public event EventHandler<EventArgs>? HeadChanged;

        public event EventHandler<BranchChanges>? BranchesCreated;

        public event EventHandler<BranchChanges>? BranchesDeleted;

        public event EventHandler<EventArgs>? BranchesModified;

        private Events() { }

        public static void RaiseWTreeChanged(object? sender = null)
            => Instance.WTreeChanged?.Invoke(sender ?? Instance, EventArgs.Empty);

        public static void RaiseWTreeCleared(object? sender = null)
            => Instance.WTreeCleared?.Invoke(sender ?? Instance, EventArgs.Empty);

        public static void RaiseHeadChanged(object? sender = null)
            => Instance.HeadChanged?.Invoke(sender ?? Instance, EventArgs.Empty);

        public static void RaiseBranchCreated(LocalBranch created, object? sender = null)
            => Instance.BranchesCreated?.Invoke(sender ?? Instance, new(created));

        public static void RaiseBranchDeleted(LocalBranch deleted, object? sender = null)
            => Instance.BranchesDeleted?.Invoke(sender ?? Instance, new(deleted));

        public static void RaiseBranchesModified(object? sender = null)
            => Instance.BranchesModified?.Invoke(sender ?? Instance, EventArgs.Empty);
    }

    public class BranchChanges : EventArgs, IEnumerable<LocalBranch> {
        public LocalBranch[] Changed { get; }
        
        public BranchChanges(params LocalBranch[] changed) => Changed = changed;

        public IEnumerator<LocalBranch> GetEnumerator() => Changed.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
