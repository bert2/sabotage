namespace hyperactive {
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class ValidateItemName : ValidationRule {
        public ValidateItemName() => ValidatesOnTargetUpdated = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
            => owner.IsDirty
                ? base.Validate(value, cultureInfo, owner)
                : ValidationResult.ValidResult;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            => value is string name
            && !File.Exists(GetPath(name))
            && !Directory.Exists(GetPath(name))
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "already exists");

        private static string GetPath(string name) {
            Debug.Assert(Repo.Instance?.WTree?.CurrentPath is not null);
            return Path.Join(Repo.Instance.WTree.CurrentPath, name);
        }
    }
}
