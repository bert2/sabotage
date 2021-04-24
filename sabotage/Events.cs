namespace sabotage {
    using System;

    using LibGit2Sharp;

    public sealed class Events {
        public static Events Instance { get; } = new Events();

        public event EventHandler<EventArgs>? WTreeModified;

        public event EventHandler<EventArgs>? WTreeCleaned;

        public event EventHandler<HeadChangedArgs>? HeadChanged;

        public event EventHandler<BranchCreatedArgs>? BranchCreated;

        public event EventHandler<BranchDeletedArgs>? BranchDeleted;

        private Events() { }

        public static void RaiseWTreeModified() => Instance.WTreeModified?.Invoke(null, EventArgs.Empty);

        public static void RaiseWTreeCleaned() => Instance.WTreeCleaned?.Invoke(null, EventArgs.Empty);

        public static void RaiseHeadChanged(OneOf<LocalBranch, Branch> newHead) => Instance.HeadChanged?.Invoke(null, new(newHead));

        public static void RaiseBranchCreated(Branch created) => Instance.BranchCreated?.Invoke(null, new(created));

        public static void RaiseBranchDeleted(LocalBranch deleted) => Instance.BranchDeleted?.Invoke(null, new(deleted));
    }

    public class BranchDeletedArgs : EventArgs {
        public LocalBranch Deleted { get; }
        public BranchDeletedArgs(LocalBranch deleted) => Deleted = deleted;
    }

    public class BranchCreatedArgs : EventArgs {
        public Branch Created { get; }
        public BranchCreatedArgs(Branch created) => Created = created;
    }

    public class HeadChangedArgs : EventArgs {
        public OneOf<LocalBranch, Branch> NewHead { get; }
        public HeadChangedArgs(OneOf<LocalBranch, Branch> newHead) => NewHead = newHead;
    }
}
