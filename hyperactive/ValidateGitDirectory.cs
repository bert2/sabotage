namespace hyperactive {
    using System.Globalization;
    using System.Windows.Controls;

    using LibGit2Sharp;

    public class ValidateGitDirectory : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            => Repository.IsValid(value?.ToString())
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Not a Git repository");
    }
}
