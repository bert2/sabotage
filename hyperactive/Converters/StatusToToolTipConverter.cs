namespace hyperactive {
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(ItemStatus), typeof(string))]
    public class StatusToToolTipConverter : IValueConverter {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            => value is ItemStatus status && parameter is ItemType type
                ? (type, status) switch {
                    (_, ItemStatus.Unchanged)                => null,

                    (ItemType.File, ItemStatus.Added)        => "added",
                    (ItemType.File, ItemStatus.Modified)     => "modified",
                    (ItemType.File, ItemStatus.Conflicted)   => "conflicted",
                    (ItemType.File, ItemStatus.Ignored)      => "ignored",

                    (ItemType.Folder, ItemStatus.Modified)   => "contents changed",
                    (ItemType.Folder, ItemStatus.Conflicted) => "conflicts in content",
                    _                                        => null
                }
                : throw new NotSupportedException();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
