namespace sabotage {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(ItemStatus), typeof(Brush))]
    public class StatusToBrushConverter : IValueConverter {
        public Brush? UnchangedBrush { get; set; }
        public Brush? AddedBrush { get; set; }
        public Brush? ModifiedBrush { get; set; }
        public Brush? ConflictedBrush { get; set; }
        public Brush? IgnoredBrush { get; set; }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            => value is ItemStatus s
                ? s switch {
                    ItemStatus.Unchanged  => UnchangedBrush,
                    ItemStatus.Added      => AddedBrush,
                    ItemStatus.Modified   => ModifiedBrush,
                    ItemStatus.Conflicted => ConflictedBrush,
                    ItemStatus.Ignored    => IgnoredBrush,
                    _                     => UnchangedBrush
                }
                : throw new NotSupportedException();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
