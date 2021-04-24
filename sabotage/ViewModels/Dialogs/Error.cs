namespace sabotage {
    public class Error {
        public string Message { get; }
        public string? Details { get; }

        public Error(string message, string? details = null) => (Message, Details) = (message, details);
    }
}
