namespace hyperactive {
    public class EnterNewItemName : ViewModel {
        public string Type { get; }

        private string? name;
        public string? Name {
            get => name;
            set {
                if (SetProperty(ref name, value))
                    Touched = true;
            }
        }

        private bool touched;
        public bool Touched { get => touched; set => SetProperty(ref touched, value); }

        public EnterNewItemName(ItemType type) => Type = $"{type.ToString().ToLower()} name";
    }
}
