using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public static class ReadOnlyCollectionsUtils
    {
        public static IReadOnlyCollection<T> SkipTake<T>(this IReadOnlyList<T> source, int skipCount, int takeCount)
        {
            var batch = new List<T>();
            
            for (int i = skipCount, currentBatchSize = 0; i < source.Count && currentBatchSize < takeCount; i++, currentBatchSize++)
            {
                batch.Add(source[i]);
            }

            return batch;
        }
    }
}
