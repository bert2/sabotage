namespace hyperactive {
    using System.Windows.Input;

    using LibGit2Sharp;

    public class RemoteBranch {
        private readonly Repository repo;

        public string Name { get; }

        public ICommand CheckoutCmd => new Command(Checkout);

        public RemoteBranch(Repo parent, string name) => (repo, Name) = (parent.LibGitRepo, name);

        private void Checkout() {
            var remoteBranch = repo.Branches[Name].NotNull();
            var localBranchName = remoteBranch.FriendlyNameWithoutRemote();

            var localBranch = repo.Branches[localBranchName]
                ?? repo.CreateBranch(localBranchName, remoteBranch.Tip);

            _ = repo.Branches.Update(localBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);

            if (!repo.Info.IsHeadUnborn) repo.Reset(ResetMode.Hard);
            Commands.Checkout(repo, localBranch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            repo.RemoveUntrackedFiles();

            Snackbar.Show("remote branch checked out");

            Events.RaiseWTreeCleaned();
            Events.RaiseHeadChanged(newHead: localBranch);
        }
    }
}
