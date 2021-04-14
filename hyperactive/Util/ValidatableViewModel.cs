namespace hyperactive {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public abstract class ValidatableViewModel : ViewModel, IDataErrorInfo {
        private readonly Dictionary<string, bool> propValid = new();

        private bool isValid;
        public bool IsValid { get => isValid; set => SetProp(ref isValid, value); }

        private bool touched;
        public bool Touched { get => touched; set => SetProp(ref touched, value); }

        public string? Error { get; }
        public string? this[string columnName] {
            get {
                if (!Touched) return null;

                var err = Validate(columnName);
                propValid[columnName] = err is null;
                IsValid = propValid.Values.All(isValid => isValid);
                return err;
            }
        }

        protected override bool SetProp<T>(ref T backingField, T value, [CallerMemberName] string? property = "") {
            var changed = base.SetProp(ref backingField, value, property);

            if (changed && property != nameof(Touched))
                Touched = true;

            return changed;
        }

        protected abstract string? Validate(string property);
    }
}
