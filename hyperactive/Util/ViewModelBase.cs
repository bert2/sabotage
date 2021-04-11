namespace hyperactive {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public abstract class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProp<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = "") {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
