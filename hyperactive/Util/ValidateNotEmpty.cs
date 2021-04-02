namespace hyperactive {
    using System.Globalization;
    using System.Windows.Controls;

    public class ValidateNotEmpty : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            return string.IsNullOrWhiteSpace(value?.ToString())
                ? new ValidationResult(false, "cannot be empty")
                : ValidationResult.ValidResult;
        }
    }
}
