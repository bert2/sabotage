
namespace hyperactive {
    using System.ComponentModel;
    using System.Linq;

    using LibGit2Sharp;

    public class EnterNewBranchName : ViewModel, IDataErrorInfo {
        private readonly Repository repo;

        public string? OldName { get; }

        private string? branchName;
        public string? BranchName {
            get => branchName;
            set {
                if (SetProp(ref branchName, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public string Error { get; } = "";
        public string this[string columnName] {
            get => columnName switch {
                nameof(BranchName) when BranchExists(repo, BranchName) => "already exists",
                _ => ""
            };
        }

        public EnterNewBranchName(LocalBranch owner, string? oldName = null)
            => (repo, OldName) = (owner.Parent.LibGitRepo, oldName);

        private static bool BranchExists(Repository repo, string? name)
            => !string.IsNullOrEmpty(name)
            && repo.Branches.Any(b => b.FriendlyName == name);
    }
}
