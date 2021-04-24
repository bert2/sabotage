namespace sabotage {
    using System.IO;

    public static class IsSubPathOfExt {
        public static bool IsSubPathOf(this string subPath, string basePath) {
            var rel = Path.GetRelativePath(basePath, subPath);
            return !rel.StartsWith('.') && !Path.IsPathRooted(rel);
        }
    }
}
