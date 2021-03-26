namespace hyperactive {
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class AndConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var v = values.OfType<IConvertible>().All(System.Convert.ToBoolean);
            return v;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
