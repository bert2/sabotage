namespace hyperactive {
    using System;
    using System.Windows.Input;

    public class Command : ICommand {
        private readonly Action execute;
        private readonly Func<bool>? canExecute;

        public event EventHandler? CanExecuteChanged;

        public Command(Action execute, Func<bool>? canExecute = null) => (this.execute, this.canExecute) = (execute, canExecute);

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
