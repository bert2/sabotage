namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using LibGit2Sharp;

    public class Repo : ViewModel {
        private string? directory = @"D:\DEV\git-conflicts"; // TODO: remove test value
        public string? Directory {
            get => directory;
            set {
                if (SetProperty(ref directory, value))
                    LoadRepository();
            }
        }

        public Repository? LibGitRepo { get; private set; }

        public static Repo? Instance { get; private set; }

        private RepoStatus status = new();
        public RepoStatus Status { get => status; private set => SetProperty(ref status, value); }

        private IBranch[]? branches;
        public IBranch[]? Branches { get => branches; private set => SetProperty(ref branches, value); }

        private int? localBranchesCount;
        public int? LocalBranchesCount { get => localBranchesCount; private set => SetProperty(ref localBranchesCount, value); }

        private int? remoteBranchesCount;
        public int? RemoteBranchesCount { get => remoteBranchesCount; private set => SetProperty(ref remoteBranchesCount, value); }

        private bool isLoaded;
        public bool IsLoaded { get => isLoaded; private set => SetProperty(ref isLoaded, value); }

        public WTreeBranch? WTree => Branches?.OfType<WTreeBranch>().Single();

        public ICommand SelectDirectoryCmd => new Command(SelectDirectory);

        public ICommand LoadRepositoryCmd => new Command(LoadRepository);

        public ICommand CheckoutBranchCmd => new Command<IBranch>(CheckoutBranch);

        public ICommand CommitCmd => new Command(Commit);

        public ICommand CreateBranchCmd => new Command<IBranch>(CreateBranch);

        public ICommand MergeBranchCmd => new Command<IBranch>(MergeBranch);

        public ICommand CherryPickCmd => new Command<IBranch>(CherryPick);

        public ICommand RenameBranchCmd => new Command<IBranch>(RenameBranch);

        public ICommand DeleteBranchCmd => new Command<IBranch>(DeleteBranch);

        public Repo() {
            WeakEventManager<Events, EventArgs>.AddHandler(Events.Instance, nameof(Events.WorkingTreeChanged), RefreshStatus);
            Instance = this;
            LoadRepository(); // TODO: remove test code
        }

        private void SelectDirectory() {
            using var dialog = new FolderBrowserDialog {
                Description = "Select Git Repository",
                UseDescriptionForTitle = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == DialogResult.OK) {
                Directory = dialog.SelectedPath;
            }
        }

        private async void LoadRepository() {
            Cleanup();
            LibGitRepo = new Repository(Directory);
            await LoadRepositoryData();
        }

        private async void CheckoutBranch(IBranch branch) {
            Debug.Assert(LibGitRepo is not null);

            LibGitRepo.Reset(ResetMode.Hard);
            Commands.Checkout(LibGitRepo, branch.Name, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            LibGitRepo.RemoveUntrackedFiles();

            Snackbar.Show("branch switched");

            await LoadRepositoryData();
        }

        private async void Commit() {
            Debug.Assert(LibGitRepo is not null);

            var (ok, message) = await Dialog.Show(new EnterCommitMessage(), vm => vm.CommitMessage);
            if (!ok) return;

            Commands.Stage(LibGitRepo, "*");

            var sig = CreateSignature(LibGitRepo);
            LibGitRepo.Commit(message, sig, sig);

            Snackbar.Show("local changes committed");

            await LoadRepositoryData();
        }

        private async void CreateBranch(IBranch source) {
            Debug.Assert(LibGitRepo is not null);

            var (ok, target) = await Dialog.Show(new EnterNewBranchName(), vm => vm.BranchName);
            if (!ok) return;

            _ = LibGitRepo.CreateBranch(branchName: target, source.Name);

            Snackbar.Show("branch created");

            await LoadRepositoryData();
        }

        private async void MergeBranch(IBranch target) {
            Debug.Assert(LibGitRepo is not null);
            Debug.Assert(Branches is not null);

            var (ok, source) = await Dialog.Show(
                new SelectMergeSource(target, sources: Branches.Where(b => b.Name != target.Name)),
                vm => vm.SelectedSource);
            if (!ok) return;

            Debug.Assert(source is not null);
            var sig = CreateSignature(LibGitRepo);
            var merge = LibGitRepo.Merge(source.Name, sig);

            Snackbar.ShowImportant(merge.Status switch {
                MergeStatus.NonFastForward => "merge succeeded",
                MergeStatus.FastForward    => "merge succeeded (fast forward)",
                MergeStatus.UpToDate       => "target branch was up-to-date",
                MergeStatus.Conflicts      => "merge failed with conflicts",
                _                          => $"merge result: {merge.Status}"
            });

            await LoadRepositoryData();
        }

        private async void CherryPick(IBranch mergeTarget) {
            Debug.Assert(LibGitRepo is not null);

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

            Debug.Assert(commit is not null);
            var sig = CreateSignature(LibGitRepo);
            var cherryPick = LibGitRepo.CherryPick(commit.GitObject, sig);

            Snackbar.ShowImportant(cherryPick.Status switch {
                CherryPickStatus.CherryPicked => "cherry-pick succeeded",
                CherryPickStatus.Conflicts => "cherry-pick failed with conflicts",
                _ => $"cherry-pick result: {cherryPick.Status}"
            });

            await LoadRepositoryData();
        }

        private async void RenameBranch(IBranch branch) {
            Debug.Assert(LibGitRepo is not null);

            var (ok, newName) = await Dialog.Show(
                new EnterNewBranchName(oldName: branch.Name),
                vm => vm.BranchName);
            if (!ok) return;

            _ = LibGitRepo.Branches.Rename(branch.Name, newName);

            Snackbar.Show("branch renamed");

            await LoadRepositoryData();
        }

        private async void DeleteBranch(IBranch branch) {
            Debug.Assert(LibGitRepo is not null);

            if (!await Dialog.Show(new Confirm(action: "delete branch", subject: branch.Name)))
                return;

            LibGitRepo.Branches.Remove(branch.Name);

            Snackbar.Show("branch deleted");

            await LoadRepositoryData();
        }

        private async void RefreshStatus(object? sender, EventArgs args) => await RefreshStatus();

        private async Task RefreshStatus() => await Task.Run(() => Status = new(LibGitRepo!));

        private async Task LoadRepositoryData() {
            Debug.Assert(LibGitRepo is not null);

            await RefreshStatus();

            Branches = LibGitRepo
                .Branches
                .Where(b => !b.IsRemote)
                .Select(b => b.IsCurrentRepositoryHead
                    ? new WTreeBranch(Directory!, b)
                    : (IBranch)new ObjDbBranch(b))
                .OrderBy(b => b.Name, Comparer<string>.Create(DevelopFirstMainLast))
                .ToArray();

            LocalBranchesCount = Branches.Length;
            RemoteBranchesCount = LibGitRepo.Branches.Count(b => b.IsRemote);

            IsLoaded = true;
        }

        private int DevelopFirstMainLast(string branch1, string branch2) => (branch1, branch2) switch {
            ("main", _)    => 1,
            (_, "main")    => -1,
            ("master", _)  => 1,
            (_, "master")  => -1,

            ("develop", _) => -1,
            (_, "develop") => 1,

            (_, _)         => branch1.CompareTo(branch2)
        };

        private void Cleanup() {
            Status = new();
            Branches = Array.Empty<IBranch>();
            LocalBranchesCount = null;
            RemoteBranchesCount = null;
            LibGitRepo?.Dispose();
        }

        private static Signature CreateSignature(Repository repo) => repo
            .Config.BuildSignature(DateTime.Now)
            ?? new Signature(new Identity("hyperactive", "hyper@active"), DateTime.Now);
    }
}
