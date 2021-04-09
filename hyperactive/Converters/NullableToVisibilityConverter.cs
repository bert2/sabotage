namespace hyperactive {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullableToVisibilityConverter : IValueConverter {
        public Visibility NullVisibility { get; set; } = Visibility.Collapsed;
        public Visibility NotNullVisibility { get; set; } = Visibility.Visible;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is null ? NullVisibility : NotNullVisibility;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
