namespace hyperactive {
    public interface IFileContent {
        public string Name { get; }
        public string Path { get; }
        public bool ReadOnly { get; }
        public string? Content { get; set; }
    }
}
