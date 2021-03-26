namespace hyperactive {
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class MainViewModel: ViewModelBase {
        private string directory = @"D:\DEV\git-conflicts";
        public string Directory {
            get => directory;
            set => SetProperty(ref directory, value);
        }

        private Repository? repo;
        public Repository? Repo {
            get => repo;
            set => SetProperty(ref repo, value, propertyName: null);
        }

        public IEnumerable<Branch>? Branches => repo?.Branches.Where(b => !b.IsRemote);
    }
}
