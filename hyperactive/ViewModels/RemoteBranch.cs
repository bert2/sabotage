namespace hyperactive {
    public class RemoteBranch {
        public Repo Repo { get; }

        public string Name { get; }

        public RemoteBranch(Repo repo, string name) => (Repo, Name) = (repo, name);
    }
}
