namespace hyperactive {
    using System.Windows.Input;

    using LibGit2Sharp;

    public abstract class LocalBranch: ViewModel {
        private readonly Repository repo;

        public Repo Parent { get; } // TODO: refactor to remove this

        private string name;
        public string Name { get => name; set => SetProp(ref name, value); }

        public bool IsHead { get; }

        private IDirectoryItem[] currentDirectory = null!;
        public IDirectoryItem[] CurrentDirectory { get => currentDirectory; protected set => SetProp(ref currentDirectory, value); }

        private IDirectoryItem? selectedItem;
        public IDirectoryItem? SelectedItem { get => selectedItem; set => SetProp(ref selectedItem, value); }

        public abstract ICommand NavigateCmd { get; }

        public abstract ICommand CreateFolderCmd { get; }

        public abstract ICommand CreateFileCmd { get; }

        public abstract ICommand RenameItemCmd { get; }

        public abstract ICommand DeleteItemCmd { get; }

        public ICommand CheckoutCmd => new Command(Checkout);

        public ICommand BranchOffCmd => new Command(BranchOff);

        public ICommand RenameCmd => new Command(Rename);

        protected LocalBranch(Repo parent, Branch branch)
            => (repo, Parent, name, IsHead) = (parent.LibGitRepo, parent, branch.FriendlyName, branch.IsCurrentRepositoryHead);

        private void Checkout() {
            repo.Reset(ResetMode.Hard);
            Commands.Checkout(repo, name, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            repo.RemoveUntrackedFiles();

            Snackbar.Show("branch switched");

            Events.RaiseWTreeChanged();
            Events.RaiseBranchesChanged();
        }

        private async void BranchOff() {
            var (ok, target) = await Dialog.Show(new EnterNewBranchName(), vm => vm.BranchName);
            if (!ok) return;

            _ = repo.CreateBranch(branchName: target, name);

            Snackbar.Show("branch created");

            Events.RaiseBranchesChanged();
        }

        private async void Rename() {
            var (ok, newName) = await Dialog.Show(
                new EnterNewBranchName(oldName: name),
                vm => vm.BranchName);
            if (!ok) return;

            _ = repo.Branches.Rename(name, newName);
            Name = newName.NotNull();

            Snackbar.Show("branch renamed");
        }

        public override string ToString() => Name;
    }
}
