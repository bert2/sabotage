namespace hyperactive.ViewModels {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LibGit2Sharp;

    using MaterialDesignExtensions.Model;

    public class GitDirSuggester : TextBoxSuggestionsSource {
        private static readonly string[] drives = DriveInfo.GetDrives().Select(d => d.Name).ToArray();

        private static readonly Regex driveLetterOnly = new("^[A-Za-z]:?$", RegexOptions.Compiled);

        public override IEnumerable<string> Search(string searchTerm)
            => GetDrivesIfEmpty(searchTerm)
            ?? GetMatchingDrivesIfDriveLetter(searchTerm)
            ?? GetNothingIfDirIsGitRepo(searchTerm)
            ?? GetSubDirsIfDir(searchTerm)
            ?? GetMatchingDirsIfIncomplete(searchTerm)
            ?? Enumerable.Empty<string>();

        private static IEnumerable<string>? GetDrivesIfEmpty(string? path)
            => string.IsNullOrWhiteSpace(path) ? drives : null;

        private static IEnumerable<string>? GetMatchingDrivesIfDriveLetter(string path)
            => driveLetterOnly.IsMatch(path)
                ? drives.Where(d => d.StartsWith(path, ignoreCase: true, culture: null))
                : null;

        private static IEnumerable<string>? GetNothingIfDirIsGitRepo(string path)
            => Path.EndsInDirectorySeparator(path) && Repository.IsValid(path)
                ? Enumerable.Empty<string>()
                : null;

        private static IEnumerable<string>? GetSubDirsIfDir(string path)
            => Directory.Exists(path) && Path.EndsInDirectorySeparator(path)
                ? Directory
                    .EnumerateDirectories(path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))
                    .Select(p => p + Path.DirectorySeparatorChar)
                : null;

        private static IEnumerable<string>? GetMatchingDirsIfIncomplete(string path) {
            var parentDir = Path.GetDirectoryName(path);
            var searchTerm = Path.GetFileName(path);
            return Directory.Exists(parentDir)
                ? Directory
                    .EnumerateDirectories(
                        parentDir!.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar),
                        $"*{searchTerm}*")
                    .OrderBy(p => p, new StartingWith(searchTerm))
                    .Select(p => p + Path.DirectorySeparatorChar)
                : null;
        }

        private class StartingWith : Comparer<string> {
            private readonly string searchTerm;

            public override int Compare(string? x, string? y) {
                if (x is null) return -1;
                if (y is null) return 1;

                x = x.ToLowerInvariant();
                y = y.ToLowerInvariant();

                var startPos = x.IndexOf(searchTerm).CompareTo(y.IndexOf(searchTerm));
                if (startPos != 0) return startPos;

                var len = x.Length.CompareTo(y.Length);
                if (len != 0) return len;

                return x.CompareTo(y);
            }

            public StartingWith(string searchTerm) => this.searchTerm = searchTerm.ToLowerInvariant();
        }
    }
}
