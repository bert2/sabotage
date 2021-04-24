namespace sabotage {
    using System;
    using System.Threading.Tasks;

    using MaterialDesignThemes.Wpf;

    public static class Dialog {
        public static async Task<(bool ok, TResult? value)> Show<TViewModel, TResult>(TViewModel viewModel, Func<TViewModel, TResult> selector)
            where TViewModel: notnull {
            var result = await DialogHost.Show(viewModel);
            var ok = result.IsOk();
            var value = ok ? selector(viewModel) : default;
            return (ok, value);
        }

        public static async Task<bool> Show<TViewModel>(TViewModel viewModel)
            where TViewModel : notnull {
            var result = await DialogHost.Show(viewModel);
            return result.IsOk();
        }

        private static bool IsOk(this object? result) => result switch {
            bool b   => b,
            not null => true,
            null     => false
        };
    }
}
