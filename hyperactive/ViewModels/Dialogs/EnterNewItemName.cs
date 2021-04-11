namespace hyperactive {
    public class EnterNewItemName : ViewModel {
        public string Type { get; }

        public string? OldName { get; }

        private string? newName;
        public string? NewName {
            get => newName;
            set {
                if (SetProp(ref newName, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public EnterNewItemName(ItemType type, string? oldName = null)
            => (Type, OldName) = (type.ToString().ToLower(), oldName);
    }
}
