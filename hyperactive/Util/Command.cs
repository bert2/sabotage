namespace hyperactive {
    using System;
    using System.Windows.Input;

    public class Command : ICommand {
        private readonly Action execute;

        public event EventHandler? CanExecuteChanged;

        public Command(Action execute) => this.execute = execute;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class Command<T> : ICommand {
        private readonly Action<T> execute;

        public event EventHandler? CanExecuteChanged;

        public Command(Action<T> execute) => this.execute = execute;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
            => execute(parameter is T t
                ? t
                : throw new InvalidOperationException($"Command parameter {parameter} must be of type {typeof(T).Name} and cannot be null."));

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class InvalidCommand : ICommand {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => false;

        public void Execute(object? parameter) => throw new InvalidOperationException();
    }
}
