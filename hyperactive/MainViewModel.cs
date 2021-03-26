namespace hyperactive {
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class MainViewModel: ViewModelBase {
        private string? directory = @"D:\DEV\git-conflicts";
        public string? Directory {
            get => directory;
            set {
                SetProperty(ref directory, value);
                Load();
            }
        }

        private Repository? repo;
        public Repository? Repo {
            get => repo;
            private set => SetProperty(ref repo, value);
        }

        private IEnumerable<Branch>? branches;
        public IEnumerable<Branch>? Branches {
            get => branches;
            private set => SetProperty(ref branches, value);
        }

        public void Load() {
            if (!Repository.IsValid(Directory)) return;

            Repo = new Repository(Directory);
            Branches = repo?.Branches.Where(b => !b.IsRemote);
        }
    }
}
