using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Provider
{
    public interface IMetadataProvider
    {
        MetadataSet Metadata { get; }
        bool TryGetMetadata(Uri uri, out IMetadataElement element);
        bool TryGetMetadata<TMetadataElement>(Uri uri, out TMetadataElement element)
            where TMetadataElement : class, IMetadataElement;
        bool TryGetMetadata<TMetadataKindIdentity>(out MetadataSet metadata) 
            where TMetadataKindIdentity : class, IMetadataKindIdentity, new();
    }
}
