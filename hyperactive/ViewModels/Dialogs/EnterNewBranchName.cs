
namespace hyperactive {
    using System.ComponentModel;
    using System.Linq;

    using LibGit2Sharp;

    public class EnterNewBranchName : ViewModel, IDataErrorInfo {
        private readonly Repository repo;

        public string? OldName { get; }

        private string? newName;
        public string? NewName {
            get => newName;
            set {
                if (SetProp(ref newName, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public string Error { get; } = "";
        public string this[string columnName] {
            get => columnName switch {
                nameof(NewName) when BranchExists(repo, NewName) => "already exists",
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
