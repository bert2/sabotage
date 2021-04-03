namespace hyperactive {
    public class Confirm {
        public string Action { get; private set; }
        public string Subject { get; private set; }

        public Confirm(string action, string subject)
            => (Action, Subject) = (action, subject);
    }
}
