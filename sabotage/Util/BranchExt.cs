namespace sabotage {
    using LibGit2Sharp;

    public static class BranchExt {
        public static string FriendlyNameWithoutRemote(this Branch branch) => branch.IsRemote
            ? branch.FriendlyName.Remove(0, branch.RemoteName.Length + 1) // +1 to strip '/' as well
            : branch.FriendlyName;
    }
}
