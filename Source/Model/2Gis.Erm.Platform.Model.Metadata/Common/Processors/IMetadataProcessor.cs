using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Processors
{
    public interface IMetadataProcessor
    {
        IMetadataKindIdentity[] TargetMetadataConstraints { get; }
        void Process(
            IMetadataKindIdentity metadataKind, 
            MetadataSet flattenedMetadata, 
            MetadataSet concreteKindMetadata, 
            IMetadataElement element);
    }
}
