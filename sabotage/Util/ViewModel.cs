namespace sabotage {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public abstract class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? property = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        protected virtual bool SetProp<T>(ref T backingField, T value, [CallerMemberName] string? property = "") {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            RaisePropertyChanged(property);
            return true;
        }
    }
}
