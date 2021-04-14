namespace hyperactive {
    using System.ComponentModel;
    using System.IO;

    public class EnterNewItemName : ViewModel, IDataErrorInfo {
        private readonly string path;

        public string Type { get; }

        public string? OldName { get; }

        private string? newName;
        public string? NewName {
            get => newName;
            set { if (SetProp(ref newName, value)) Touched = true; }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public string Error { get; } = "";
        public string this[string columnName] {
            get => columnName switch {
                _ when !Touched => "",
                nameof(NewName) when string.IsNullOrWhiteSpace(NewName) => "cannot be empty",
                nameof(NewName) when FileExists(path, NewName) => "already exists",
                _ => ""
            };
        }

        public EnterNewItemName(WTreeBranch owner, ItemType type, string? oldName = null)
            => (path, Type, OldName) = (owner.CurrentPath, type.ToString().ToLower(), oldName);

        private static bool FileExists(string path, string newName) {
            var fullName = Path.Join(path, newName);
            return File.Exists(fullName) || Directory.Exists(fullName);
        }
    }
}
