using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Operations.Extensions
{
    internal static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int batchSize)
        {
            var buffer = new List<T>(batchSize);

            foreach (var item in items)
            {
                buffer.Add(item);

                if (buffer.Count == buffer.Capacity)
                {
                    yield return buffer;
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                yield return buffer;
            }
        }
    }
}