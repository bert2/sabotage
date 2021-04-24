namespace sabotage {
    using System;

    using LibGit2Sharp;

    public class Commit_ {
        public Commit GitObject { get; }

        public string Sha { get; }

        public string ShortSha { get; }

        public string Message { get; }

        public string ShortMessage { get; }

        public Commit_(Commit c) => (GitObject, Sha, ShortSha, Message, ShortMessage) = (c, c.Sha, c.Sha.Substring(0, 7), c.Message, c.MessageShort);

        public bool Matches(string shaOrMessage)
            => string.IsNullOrWhiteSpace(shaOrMessage)
            || Sha.Contains(shaOrMessage, StringComparison.InvariantCultureIgnoreCase)
            || Message.Contains(shaOrMessage, StringComparison.InvariantCultureIgnoreCase);

        public override string ToString() => $"{ShortSha} {ShortMessage}";
    }
}
