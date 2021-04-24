namespace sabotage {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : IValueConverter {
        public Brush? TrueBrush { get; set; }

        public Brush? FalseBrush { get; set; }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b ? TrueBrush : FalseBrush;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
