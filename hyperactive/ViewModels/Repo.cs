namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using LibGit2Sharp;

    using MoreLinq;

    public sealed class Repo : ViewModel, IDisposable {
        public static Repo? Instance { get; private set; } // TODO: refactor

        public string Path { get; }

        public Repository LibGitRepo { get; }

        private Status status = new();
        public Status Status { get => status; private set => SetProp(ref status, value); }

        private ObservableCollection<LocalBranch> branches = null!;
        public ObservableCollection<LocalBranch> Branches { get => branches; private set => SetProp(ref branches, value); }

        private RemoteBranch[] remoteBranches = null!;
        public RemoteBranch[] RemoteBranches { get => remoteBranches; private set => SetProp(ref remoteBranches, value); }

        public WTreeBranch Head => Branches.OfType<WTreeBranch>().Single();

        public ICommand MergeBranchCmd => new Command<LocalBranch>(MergeBranch);

        public ICommand CherryPickCmd => new Command<LocalBranch>(CherryPick);

        public Repo(string path) {
            Path = path;
            LibGitRepo = new Repository(path);
            LoadRepositoryData();

            Events.Instance.WTreeChanged += RefreshStatus;
            Events.Instance.WTreeCleared += ResetStatus;
            Events.Instance.HeadChanged += RefreshHead;
            Events.Instance.BranchesCreated += AddBranches;
            Events.Instance.BranchesDeleted += RemoveBranches;
            Events.Instance.BranchesModified += RefreshBranches;

            Instance = this; // TODO: refactor
        }

        public void Dispose() {
            Events.Instance.WTreeChanged -= RefreshStatus;
            Events.Instance.WTreeCleared -= ResetStatus;
            Events.Instance.HeadChanged -= RefreshHead;
            Events.Instance.BranchesCreated -= AddBranches;
            Events.Instance.BranchesDeleted -= RemoveBranches;
            Events.Instance.BranchesModified -= RefreshBranches;

            LibGitRepo.Dispose();

            Instance = null; // TODO: refactor
        }

        private void LoadRepositoryData() {
            LoadStatus();
            LoadLocalBranches();
            LoadRemoteBranches();
        }

        private void LoadStatus() => Status = new(LibGitRepo);

        private void LoadLocalBranches()
            => Branches = new(LibGitRepo
                .Branches
                .Where(b => !b.IsRemote)
                .Select<Branch, LocalBranch>(b => b.IsCurrentRepositoryHead
                    ? new WTreeBranch(this, b)
                    : new ObjDbBranch(this, b))
                .OrderBy(b => b.Name, developFirstMainLast));

        private void LoadRemoteBranches()
            => RemoteBranches = LibGitRepo
                .Branches
                .Where(b
                    => b.IsRemote
                    && b.Reference is DirectReference) // skips refs like "remotes/origin/HEAD -> origin/master"
                .Select(b => new RemoteBranch(this, b.FriendlyName))
                .OrderBy(b => b.Name, developFirstMainLast)
                .ToArray();

        private void RefreshStatus(object? sender, EventArgs args) => LoadStatus();

        private void ResetStatus(object? sender, EventArgs args) => Status = new();

        private void RefreshHead(object? sender, EventArgs args) => RaisePropertyChanged(nameof(Head));

        private void AddBranches(object? sender, BranchChanges created)
            => created.ForEach(b => Branches.InsertSorted(b, DevelopFirstMainLast));

        private void RemoveBranches(object? sender, BranchChanges deleted) => deleted.ForEach(b => Branches.Remove(b));

        private void RefreshBranches(object? sender, EventArgs args) => LoadLocalBranches();

        private async void MergeBranch(LocalBranch target) {
            var (ok, source) = await Dialog.Show(
                new SelectMergeSource(target, sources: Branches.Where(b => b.Name != target.Name)),
                vm => vm.SelectedSource);
            if (!ok) return;

            var merge = LibGitRepo.Merge(source.NotNull().Name, LibGitRepo.CreateSignature());

            Snackbar.ShowImportant(merge.Status switch {
                MergeStatus.NonFastForward => "merge succeeded",
                MergeStatus.FastForward    => "merge succeeded (fast forward)",
                MergeStatus.UpToDate       => "target branch was up-to-date",
                MergeStatus.Conflicts      => "merge failed with conflicts",
                _                          => $"merge result: {merge.Status}"
            });

            LoadStatus();
            Head.ReloadCurrentFolder();
        }

        private async void CherryPick(LocalBranch mergeTarget) {
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

            var cherryPick = LibGitRepo.CherryPick(commit.NotNull().GitObject, LibGitRepo.CreateSignature());

            Snackbar.ShowImportant(cherryPick.Status switch {
                CherryPickStatus.CherryPicked => "cherry-pick succeeded",
                CherryPickStatus.Conflicts => "cherry-pick failed with conflicts",
                _ => $"cherry-pick result: {cherryPick.Status}"
            });

            LoadStatus();
            Head.ReloadCurrentFolder();
        }

        private static readonly IComparer<string> developFirstMainLast = Comparer<string>.Create(DevelopFirstMainLast);

        private static int DevelopFirstMainLast(LocalBranch x, LocalBranch y) => DevelopFirstMainLast(x.Name, y.Name);

        private static int DevelopFirstMainLast(string x, string y) => (x, y) switch {
            ("main"   , _        ) =>  1,
            (_        , "main"   ) => -1,
            ("master" , _        ) =>  1,
            (_        , "master" ) => -1,

            ("develop", _        ) => -1,
            (_        , "develop") =>  1,

            (_        , _        ) => x.CompareTo(y)
        };
    }
}
