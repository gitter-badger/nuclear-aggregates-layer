using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities
{
    /// <summary>
    /// Набор hepler методов для работы с Id элемента метаданных, используя различные виды входных данных
    /// В текущей реализации главное - это инвариантное поведение, независимо от вариаций входных данных, (например, есть или нет leading и trailing / в аргументах), 
    /// если же использовать класс Uri для комбирирования сегментов непосредственно (различные конструкторы и т.п.), то  инвариантности уже нет - все определяется входными данными 
    /// </summary>
    public static class IdBuilder
    {
        private const char UriSeparator = '/';
        private const string MetadataBaseUri = @"erm://metadata";
        private const string StubUriTemplate = MetadataBaseUri + "/STUB_";

        public static Uri StubUnique
        {
            get
            {
                return new Uri(StubUriTemplate + IncreasingSequenceGenerator.Next);
            }
        }

        public static bool IsStub(this Uri id)
        {
            if (!id.IsAbsoluteUri)
            {
                return false;
            }

            var regex = new Regex(StubUriTemplate + @"\d+");
            return regex.IsMatch(id.AbsoluteUri);
        }

        public static Uri For(params string[] segments)
        {
            return MetadataBaseUri.ToId(segments);
        }

        public static Uri For<TMetadataKindIdentity>(params string[] segments)
            where TMetadataKindIdentity : MetadataKindIdentityBase<TMetadataKindIdentity>, new()
        {
            var metadataKindIdentity = new TMetadataKindIdentity();
            if (metadataKindIdentity.Id == null || !metadataKindIdentity.Id.IsAbsoluteUri)
            {
                throw new InvalidOperationException("Specified metadataKindIdentity must have Absolute Uri Id");
            }

            return metadataKindIdentity.Id.AbsoluteUri.ToId(segments);
        }

        public static Uri UniqueFor(params string[] segments)
        {
            var extendedSegments = new string[segments.Length + 1];
            segments.CopyTo(extendedSegments, 0);
            extendedSegments[extendedSegments.Length - 1] = IncreasingSequenceGenerator.Next.ToString();
            return For(extendedSegments);
        }

        public static Uri DynamicChild(this Uri parentId)
        {
            if (parentId == null || !parentId.IsAbsoluteUri)
            {
                throw new ArgumentException("parentId");
            }

            return parentId.AbsoluteUri.ToId("Child_" + IncreasingSequenceGenerator.Next);
        }

        public static Uri WithRelative(this Uri parentId, Uri relativeId)
        {
            if (parentId == null || !parentId.IsAbsoluteUri)
            {
                throw new ArgumentException("parentId");
            }

            if (relativeId == null || relativeId.IsAbsoluteUri)
            {
                throw new ArgumentException("relativeId");
            }

            return parentId.AbsoluteUri.ToId(relativeId.OriginalString);
        }

        public static Uri IdFor<TMetadataKindIdentity>(this StrictOperationIdentity operation)
            where TMetadataKindIdentity : MetadataKindIdentityBase<TMetadataKindIdentity>, new()
        {
            var metadataKindIdentity = new TMetadataKindIdentity();
            if (metadataKindIdentity.Id == null || !metadataKindIdentity.Id.IsAbsoluteUri)
            {
                throw new InvalidOperationException("Specified metadataKindIdentity must have Absolute Uri Id");
            }

            return metadataKindIdentity.Id.AbsoluteUri.ToId(operation.AsUriSegment());
        }

        public static Uri IdFor<TEntity>(this IMetadataKindIdentity metadataKindIdentity)
            where TEntity : class, IEntity
        {
            var entityName = typeof(TEntity).AsEntityName();
            if (metadataKindIdentity.Id == null || !metadataKindIdentity.Id.IsAbsoluteUri)
            {
                throw new InvalidOperationException("Specified metadataKindIdentity must have Absolute Uri Id");
            }

            return metadataKindIdentity.Id.AbsoluteUri.ToId(entityName.ToString());
        }

        public static IMetadataElementIdentity AsIdentity(this Uri uri)
        {
            return new MetadataElementIdentity(uri);
        }

        private static Uri ToId(this string basePart, params string[] segments)
        {
            var uriValue = 
                segments
                    .Aggregate(new StringBuilder(basePart.Trim(UriSeparator)), (sb, s) => sb.Append(UriSeparator + s.Trim(UriSeparator)))
                    .ToString();
            return new Uri(uriValue);
        }
    }
}
