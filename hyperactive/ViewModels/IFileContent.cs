namespace hyperactive {
    public interface IFileContent {
        public bool ReadOnly { get; }
        public string? Content { get; set; }
    }
}
