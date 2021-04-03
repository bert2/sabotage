namespace hyperactive {
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class ValidateBranchName : ValidationRule {
        public ValidateBranchName() => ValidatesOnTargetUpdated = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
            => owner.IsDirty
                ? base.Validate(value, cultureInfo, owner)
                : ValidationResult.ValidResult;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            Debug.Assert(Repo.Current is not null);
            return value is string branchName && Repo.Current.Branches.All(b => b.FriendlyName != branchName)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "already taken");
        }
    }
}
