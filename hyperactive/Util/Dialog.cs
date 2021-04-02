namespace hyperactive {
    using System;
    using System.Threading.Tasks;

    using MaterialDesignThemes.Wpf;

    public static class Dialog {
        public static async Task<(bool ok, TResult? value)> Show<TViewModel, TResult>(TViewModel viewModel, Func<TViewModel, TResult> selector)
            where TViewModel: notnull {
            var result = await DialogHost.Show(viewModel);
            var ok = (result is bool b && b) || (result is not null);
            var value = ok ? selector(viewModel) : default;
            return (ok, value);
        }
    }
}
