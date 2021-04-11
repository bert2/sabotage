namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using LibGit2Sharp;

    public sealed class Repo : ViewModel, IDisposable {
        public static Repo? Instance { get; private set; } // TODO: refactor

        public string Path { get; }

        public Repository LibGitRepo { get; }

        private RepoStatus status = new();
        public RepoStatus Status { get => status; private set => SetProp(ref status, value); }

        private IBranch[] branches = null!;
        public IBranch[] Branches { get => branches; private set => SetProp(ref branches, value); }

        private RemoteBranch[] remoteBranches = null!;
        public RemoteBranch[] RemoteBranches { get => remoteBranches; private set => SetProp(ref remoteBranches, value); }

        public WTreeBranch WTree => Branches.OfType<WTreeBranch>().Single();

        public ICommand CheckoutBranchCmd => new Command<IBranch>(CheckoutBranch);

        public ICommand CheckoutRemoteBranchCmd => new Command<RemoteBranch>(CheckoutRemoteBranch);

        public ICommand CommitCmd => new Command(Commit);

        public ICommand CreateBranchCmd => new Command<IBranch>(CreateBranch);

        public ICommand MergeBranchCmd => new Command<IBranch>(MergeBranch);

        public ICommand CherryPickCmd => new Command<IBranch>(CherryPick);

        public ICommand RenameBranchCmd => new Command<IBranch>(RenameBranch);

        public ICommand DeleteBranchCmd => new Command<IBranch>(DeleteBranch);

        public Repo(string path) {
            Path = path;
            LibGitRepo = new Repository(path);
            LoadRepositoryData();
            WeakEventManager<Events, EventArgs>.AddHandler(
                Events.Instance,
                nameof(Events.WorkingTreeChanged),
                (_, __) => Status = new(LibGitRepo));
            Instance = this; // TODO: refactor
        }

        private void LoadRepositoryData() {
            Status = new(LibGitRepo);

            Branches = LibGitRepo
                .Branches
                .Where(b => !b.IsRemote)
                .Select(b => b.IsCurrentRepositoryHead
                    ? new WTreeBranch(this, b)
                    : (IBranch)new ObjDbBranch(this, b))
                .OrderBy(b => b.Name, sortDevelopFirstMainLast)
                .ToArray();

            RemoteBranches = LibGitRepo
                .Branches
                .Where(b
                    => b.IsRemote
                    && b.Reference is DirectReference) // skips refs like "remotes/origin/HEAD -> origin/master"
                .Select(b => new RemoteBranch(this, b.FriendlyName))
                .OrderBy(b => b.Name, sortDevelopFirstMainLast)
                .ToArray();
        }

        public void Dispose() {
            LibGitRepo.Dispose();
            Instance = null; // TODO: refactor
        }

        private void CheckoutBranch(IBranch branch) {
            LibGitRepo.Reset(ResetMode.Hard);
            Commands.Checkout(LibGitRepo, branch.Name, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            LibGitRepo.RemoveUntrackedFiles();

            Snackbar.Show("branch switched");

            LoadRepositoryData();
        }

        private void CheckoutRemoteBranch(RemoteBranch remoteBranch_) {
            var remoteBranch = LibGitRepo.Branches[remoteBranch_.Name].NotNull();
            var localBranchName = remoteBranch.FriendlyNameWithoutRemote();

            var localBranch = LibGitRepo.Branches[localBranchName]
                ?? LibGitRepo.CreateBranch(localBranchName, remoteBranch.Tip);

            _ = LibGitRepo.Branches.Update(localBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);

            LibGitRepo.Reset(ResetMode.Hard);
            Commands.Checkout(LibGitRepo, localBranch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            LibGitRepo.RemoveUntrackedFiles();

            Snackbar.Show("remote branch checked out");

            LoadRepositoryData();
        }

        private async void Commit() {
            var (ok, message) = await Dialog.Show(new EnterCommitMessage(), vm => vm.CommitMessage);
            if (!ok) return;

            Commands.Stage(LibGitRepo, "*");

            var sig = CreateSignature();
            LibGitRepo.Commit(message, sig, sig);

            Snackbar.Show("local changes committed");

            LoadRepositoryData();
        }

        private async void CreateBranch(IBranch source) {
            var (ok, target) = await Dialog.Show(new EnterNewBranchName(), vm => vm.BranchName);
            if (!ok) return;

            _ = LibGitRepo.CreateBranch(branchName: target, source.Name);

            Snackbar.Show("branch created");

            LoadRepositoryData();
        }

        private async void MergeBranch(IBranch target) {
            var (ok, source) = await Dialog.Show(
                new SelectMergeSource(target, sources: Branches.Where(b => b.Name != target.Name)),
                vm => vm.SelectedSource);
            if (!ok) return;

            var merge = LibGitRepo.Merge(source.NotNull().Name, CreateSignature());

            Snackbar.ShowImportant(merge.Status switch {
                MergeStatus.NonFastForward => "merge succeeded",
                MergeStatus.FastForward    => "merge succeeded (fast forward)",
                MergeStatus.UpToDate       => "target branch was up-to-date",
                MergeStatus.Conflicts      => "merge failed with conflicts",
                _                          => $"merge result: {merge.Status}"
            });

            LoadRepositoryData();
        }

        private async void CherryPick(IBranch mergeTarget) {
            var commits = LibGitRepo
                .Commits
                .QueryBy(new CommitFilter {
                    ExcludeReachableFrom = mergeTarget.Name,
                    IncludeReachableFrom = LibGitRepo.Branches.Where(b => b.FriendlyName != mergeTarget.Name),
                    SortBy = CommitSortStrategies.Time
                })
                .Select(c => new Commit_(c))
                .ToArray();

            var (ok, commit) = await Dialog.Show(new SelectCommit(mergeTarget, commits), vm => vm.SelectedCommit);
            if (!ok) return;

            var cherryPick = LibGitRepo.CherryPick(commit.NotNull().GitObject, CreateSignature());

            Snackbar.ShowImportant(cherryPick.Status switch {
                CherryPickStatus.CherryPicked => "cherry-pick succeeded",
                CherryPickStatus.Conflicts => "cherry-pick failed with conflicts",
                _ => $"cherry-pick result: {cherryPick.Status}"
            });

            LoadRepositoryData();
        }

        private async void RenameBranch(IBranch branch) {
            var (ok, newName) = await Dialog.Show(
                new EnterNewBranchName(oldName: branch.Name),
                vm => vm.BranchName);
            if (!ok) return;

            _ = LibGitRepo.Branches.Rename(branch.Name, newName);

            Snackbar.Show("branch renamed");

            LoadRepositoryData();
        }

        private async void DeleteBranch(IBranch branch) {
            if (!await Dialog.Show(new Confirm(action: "delete branch", subject: branch.Name)))
                return;

            LibGitRepo.Branches.Remove(branch.Name);

            Snackbar.Show("branch deleted");

            LoadRepositoryData();
        }

        private static readonly IComparer<string> sortDevelopFirstMainLast = Comparer<string>.Create(DevelopFirstMainLast);

        private static int DevelopFirstMainLast(string branch1, string branch2) => (branch1, branch2) switch {
            ("main", _)    =>  1,
            (_, "main")    => -1,
            ("master", _)  =>  1,
            (_, "master")  => -1,

            ("develop", _) => -1,
            (_, "develop") =>  1,

            (_, _)         => branch1.CompareTo(branch2)
        };

        private Signature CreateSignature()
            => LibGitRepo.Config.BuildSignature(DateTime.Now)
            ?? new Signature(new Identity("hyperactive", "hyper@active"), DateTime.Now);
    }
}
