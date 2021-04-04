namespace hyperactive {
    public class Confirm {
        public string Action { get; }
        public string Subject { get; }

        public Confirm(string action, string subject) => (Action, Subject) = (action, subject);
    }
}
