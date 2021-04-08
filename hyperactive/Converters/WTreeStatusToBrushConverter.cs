namespace hyperactive {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(WTreeStatus), typeof(Brush))]
    public class WTreeStatusToBrushConverter : IValueConverter {
        public Brush? CleanBrush { get; set; }
        public Brush? DirtyBrush { get; set; }
        public Brush? ConflictedBrush { get; set; }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            => value is WTreeStatus s
                ? s switch {
                    WTreeStatus.Clean      => CleanBrush,
                    WTreeStatus.Dirty      => DirtyBrush,
                    WTreeStatus.Conflicted => ConflictedBrush,
                    _                      => CleanBrush
                }
                : CleanBrush;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
