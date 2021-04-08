namespace hyperactive {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToColorConverter : IValueConverter {
        /// <summary>Converts a boolean to a SolidColorBrush.</summary>
        /// <param name="value">Bolean value controlling wether to apply color change.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Optional CSV string specifying the color names for both cases. Default is "DarkGreen;DarkRed".</param>
        /// <param name="culture"></param>
        /// <returns>A SolidColorBrush.</returns>
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture) {
            var colorIfTrue = Colors.Green;
            var colorIfFalse = Colors.Red;

            var names = parameter?.ToString()?.Split(';');

            if (names?.Length > 0 && !string.IsNullOrEmpty(names[0])) {
                colorIfTrue = ColorFromName(names[0]);
            }

            if (names?.Length > 1 && !string.IsNullOrEmpty(names[1])) {
                colorIfFalse = ColorFromName(names[1]);
            }

            return new SolidColorBrush((bool)value ? colorIfTrue : colorIfFalse);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();

        private static Color ColorFromName(string name) {
            var c = System.Drawing.Color.FromName(name);
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}
