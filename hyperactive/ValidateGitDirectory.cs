namespace hyperactive {
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    using LibGit2Sharp;

    public class ValidateGitDirectory : ValidationRule {
        public ValidateGitDirectory() => ValidatesOnTargetUpdated = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
            => owner.IsDirty
                ? base.Validate(value, cultureInfo, owner)
                : ValidationResult.ValidResult;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            => value is string path && Repository.IsValid(path)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "not a git repository");
    }
}
