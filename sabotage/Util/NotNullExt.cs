namespace sabotage {
    using System;

    public static class NotNullExt {
        public static T NotNull<T>(this T? x) => x is null
            ? throw new InvalidOperationException($"Value of type {typeof(T).Name} was unexpectedly null.")
            : x;
    }
}
