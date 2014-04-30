using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Processors.Concrete
{
    public sealed class Feature2PropertiesLinkerProcessor : MetadataProcessorBase<MetadataEntitiesIdentity, EntityPropertyMetadata>
    {
        protected override void Process(
            IMetadataKindIdentity metadataKind,
            MetadataSet flattenedMetadata,
            MetadataSet concreteKindMetadata,
            EntityPropertyMetadata metadata)
        {
            foreach (var propertyFeature in metadata.Features.OfType<IPropertyFeature>())
            {
                propertyFeature.TargetPropertyMetadata = metadata;
            }
        }
    }
}