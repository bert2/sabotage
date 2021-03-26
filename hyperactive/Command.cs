namespace hyperactive {
    using System;
    using System.Windows.Input;

    public class Command : ICommand {
        private readonly Action execute;

        public event EventHandler? CanExecuteChanged;

        public Command(Action execute) => this.execute = execute;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute();
    }
}
