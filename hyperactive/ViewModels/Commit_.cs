namespace hyperactive {
    using LibGit2Sharp;

    public class Commit_ {
        public Commit GitObject { get; }

        public string ShortSha { get; }

        public string Message { get; }

        public Commit_(Commit c) => (GitObject, ShortSha, Message) = (c, c.Sha.Substring(0, 7), c.MessageShort);

        public override string ToString() => $"{ShortSha} {Message}";
    }
}
