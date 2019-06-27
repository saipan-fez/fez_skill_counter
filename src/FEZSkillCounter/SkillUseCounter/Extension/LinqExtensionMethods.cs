using System;
using System.Collections.Generic;
using System.Linq;

namespace FEZSkillUseCounter.Extension
{
    public static class LinqExtensionMethods
    {
        private class CompareSelector<T, TKey> : IEqualityComparer<T>
        {
            private Func<T, TKey> selector;

            public CompareSelector(Func<T, TKey> selector)
            {
                this.selector = selector;
            }

            public bool Equals(T x, T y)
            {
                return selector(x).Equals(selector(y));
            }

            public int GetHashCode(T obj)
            {
                return selector(obj).GetHashCode();
            }
        }

        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.Distinct(new CompareSelector<T, TKey>(selector));
        }

        public static bool SequenceEqual<T, TKey>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, TKey> selector)
        {
            return source.SequenceEqual(second, new CompareSelector<T, TKey>(selector));
        }

        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, TKey> selector)
        {
            return source.Except(second, new CompareSelector<T, TKey>(selector));
        }
    }
}
