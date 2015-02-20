using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public static class EnumerableExtensions
    {
        public static T[] AsArray<T>(this IEnumerable<T> source)
        {
            return source as T[] ?? source.ToArray();
        }

        public static IEnumerable<T> With<T>(this IEnumerable<T> source, params T[] additionalElements)
        {
            return source.Concat(additionalElements);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector,
                                                                     IEqualityComparer<TKey> comparer)
        {
            return DistinctByImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
                                                                          Func<TSource, TKey> keySelector,
                                                                          IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}