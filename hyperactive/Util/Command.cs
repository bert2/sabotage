namespace hyperactive {
    using System;
    using System.Windows.Input;

    public class Command : ICommand {
        private readonly Action execute;

        public event EventHandler? CanExecuteChanged {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        public Command(Action execute) => this.execute = execute;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute();
    }

    public class Command<T> : ICommand {
        private readonly Action<T> execute;

        public event EventHandler? CanExecuteChanged {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        public Command(Action<T> execute) => this.execute = execute;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
            => execute(parameter is T t
                ? t
                : throw new InvalidOperationException($"Command parameter {parameter} must be of type {typeof(T).Name} and cannot be null."));
    }

    public class InvalidCommand : ICommand {
        public event EventHandler? CanExecuteChanged {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        public bool CanExecute(object? parameter) => false;

        public void Execute(object? parameter) => throw new InvalidOperationException();
    }
}
