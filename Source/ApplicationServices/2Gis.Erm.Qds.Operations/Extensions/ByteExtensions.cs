using System;
using System.Globalization;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Extensions
{
    internal static class IndexerExtensions
    {
        private static readonly string External = VersionType.External.ToString().ToLowerInvariant();

        public static BulkIndexDescriptor<TDocument> Version<TDocument>(this BulkIndexDescriptor<TDocument> descriptor, byte[] timestamp, bool indirectly)
            where TDocument : class
        {
            if (indirectly)
            {
                return descriptor;
            }

            // умножаем timestamp на 10000 чтобы отличать косвенные апдейты документов от прямых
            // косвенный даёт +1, прямой даёт *10000
            // если не различать косвенные и прямые апдейты, то тогда после косвенного апдейта невозможно сделать прямой - оптимистичная блокировка
            var version = timestamp.ToUInt64() * 10000;

            descriptor
                .Version(version.ToString(CultureInfo.InvariantCulture))
                .VersionType(External);

            return descriptor;
        }

        private static ulong ToUInt64(this byte[] bytes)
        {
            if (bytes.Length != 8)
            {
                throw new Exception();
            }

            ulong result = 0;
            result |= bytes[7];
            result |= (ulong)bytes[6] << 8;
            result |= (ulong)bytes[5] << 16;
            result |= (ulong)bytes[4] << 24;
            result |= (ulong)bytes[3] << 32;
            result |= (ulong)bytes[2] << 40;
            result |= (ulong)bytes[1] << 48;
            result |= (ulong)bytes[0] << 56;

            return result;
        }
    }
}