namespace sabotage {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(int), typeof(bool))]
    public class ZeroToTrueConverter : IValueConverter {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is int i && i == 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
