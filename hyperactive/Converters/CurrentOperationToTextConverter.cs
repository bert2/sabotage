namespace hyperactive {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using LibGit2Sharp;

    [ValueConversion(typeof(CurrentOperation), typeof(string))]
    public class CurrentOperationToTextConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            => value is CurrentOperation op
                ? op switch {
                    CurrentOperation.Merge                => "(merge in progress)",
                    CurrentOperation.CherryPick           => "(cherry-pick in progress)",
                    CurrentOperation.CherryPickSequence   => "(cherry-pick in progress)",
                    CurrentOperation.Revert               => "(revert in progress)",
                    CurrentOperation.RevertSequence       => "(revert in progress)",
                    CurrentOperation.Rebase               => "(rebase in progress)",
                    CurrentOperation.RebaseInteractive    => "(rebase in progress)",
                    CurrentOperation.RebaseMerge          => "(rebase in progress)",
                    CurrentOperation.Bisect               => "(bisect in progress)",
                    CurrentOperation.ApplyMailbox         => "(apply in progress)",
                    CurrentOperation.ApplyMailboxOrRebase => "(apply in progress)",
                    CurrentOperation.None                 => "",
                    _                                     => ""
                }
                : throw new NotSupportedException();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
