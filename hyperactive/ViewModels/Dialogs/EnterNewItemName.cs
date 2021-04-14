namespace hyperactive {
    using System.IO;

    public class EnterNewItemName : ValidatableViewModel {
        private readonly string path;

        public string Type { get; }

        public string? OldName { get; }

        private string? newName;
        public string? NewName { get => newName; set => SetProp(ref newName, value); }

        public EnterNewItemName(WTreeBranch owner, ItemType type, string? oldName = null)
            => (path, Type, OldName) = (owner.CurrentPath, type.ToString().ToLower(), oldName);

        protected override string? Validate(string property) => property switch {
            nameof(NewName) when string.IsNullOrWhiteSpace(NewName) => "cannot be empty",
            nameof(NewName) when FileExists(path, NewName) => "already exists",
            _ => null
        };

        private static bool FileExists(string path, string newName) {
            var fullName = Path.Join(path, newName);
            return File.Exists(fullName) || Directory.Exists(fullName);
        }
    }
}
