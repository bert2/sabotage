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

    using MaterialDesignThemes.Wpf;

    public class Repo : ViewModel {
        private string? directory = @"D:\DEV\git-conflicts"; // TODO: remove test value
        public string? Directory {
            get => directory;
            set {
                SetProperty(ref directory, value);
                LoadRepository();
            }
        }

        private Repository? repo;

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

        public ICommand SelectDirectoryCmd => new Command(SelectDirectory);

        public ICommand LoadRepositoryCmd => new Command(LoadRepository);

        public ICommand CheckoutBranchCmd => new Command<IBranch>(CheckoutBranch);

        public ICommand CommitCmd => new Command(Commit);

        public ICommand MergeBranchCmd => new Command<IBranch>(MergeBranch);

        public ICommand CreateBranchCmd => new Command<IBranch>(CreateBranch);

        public Repo() {
            WeakEventManager<Events, EventArgs>.AddHandler(Events.Instance, nameof(Events.WorkingTreeChanged), RefreshStatus);
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
            repo = new Repository(Directory);
            await LoadRepositoryData();
        }

        private async void CheckoutBranch(IBranch branch) {
            Debug.Assert(repo is not null);

            repo.Reset(ResetMode.Hard);
            repo.RemoveUntrackedFiles();
            Commands.Checkout(repo, branch.Name, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });

            await LoadRepositoryData();
        }

        private async Task LoadRepositoryData() {
            Debug.Assert(repo is not null);

            await RefreshStatus();

            Branches = repo
                .Branches
                .Where(b => !b.IsRemote)
                .Select(b => b.IsCurrentRepositoryHead
                    ? new WTreeBranch(Directory!, b)
                    : (IBranch)new ObjDbBranch(b))
                .OrderBy(b => b.Name, Comparer<string>.Create(DevelopFirstMainLast))
                .ToArray();

            LocalBranchesCount = Branches.Length;
            RemoteBranchesCount = repo.Branches.Count(b => b.IsRemote);

            IsLoaded = true;
        }

        private async void Commit() {
            Debug.Assert(repo is not null);

            var (ok, message) = await Dialog.Show(new EnterCommitMessage(), vm => vm.CommitMessage);
            if (!ok) return;

            Commands.Stage(repo, "*");

            var sig = CreateSignature(repo);
            repo.Commit(message, sig, sig);

            Snackbar.Show("local changes committed");

            await RefreshStatus();
        }

        private async void MergeBranch(IBranch target) {
            Debug.Assert(repo is not null);
            Debug.Assert(Branches is not null);

            var (ok, source) = await Dialog.Show(
                new SelectMergeSource(target, sources: Branches.Where(b => b.Name != target.Name)),
                vm => vm.SelectedSource);
            if (!ok) return;

            var sig = CreateSignature(repo);
            var merge = repo.Merge(source!.Name, sig);

            Snackbar.ShowImportant(merge.Status switch {
                MergeStatus.NonFastForward => "merge succeeded",
                MergeStatus.FastForward => "merge succeeded (fast forward)",
                MergeStatus.UpToDate => "target branch was up-to-date",
                MergeStatus.Conflicts => "merge failed with conflicts",
                _ => $"merge result: {merge.Status}"
            });

            await LoadRepositoryData();
        }

        private async void CreateBranch(IBranch source) {
            Debug.Assert(repo is not null);
            Debug.Assert(Branches is not null);

            var (ok, target) = await Dialog.Show(new EnterNewBranchName(), vm => vm.BranchName);
            if (!ok) return;

            _ = repo.CreateBranch(branchName: target, source.Name);

            Snackbar.Show("branch created");

            await LoadRepositoryData();
        }

        private async void RefreshStatus(object? sender, EventArgs args) => await RefreshStatus();

        private async Task RefreshStatus() => await Task.Run(() => Status = new(repo!));

        private int DevelopFirstMainLast(string branch1, string branch2) => (branch1, branch2) switch {
            ("main"   , _        ) =>  1,
            (_        , "main"   ) => -1,
            ("master" , _        ) =>  1,
            (_        , "master" ) => -1,

            ("develop", _        ) => -1,
            (_        , "develop") =>  1,

            (_        , _        ) => branch1.CompareTo(branch2)
        };

        private void Cleanup() {
            Status = new();
            Branches = Array.Empty<IBranch>();
            LocalBranchesCount = null;
            RemoteBranchesCount = null;
            repo?.Dispose();
        }

        private static Signature CreateSignature(Repository repo) => repo
            .Config.BuildSignature(DateTime.Now)
            ?? new Signature(new Identity("hyperactive", "hyper@active"), DateTime.Now);
    }
}
