namespace hyperactive {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MoreLinq;

    public static class ListExt {
        public static void InsertSorted<T>(this IList<T> xs, T x, Comparison<T> comparison) {
            var index = xs
                .Index()
                .Where(y => comparison(y.Value, x) > 0)
                .Select(y => (int?)y.Key)
                .FirstOrDefault()
                ?? xs.Count;

            xs.Insert(index, x);
        }
    }
}
