using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Entities;
using NuClear.Metamodeling.Domain.Entities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities
{
    public sealed class EntitiesMetadataSource : MetadataSourceBase<MetadataEntitiesIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public EntitiesMetadataSource()
        {
            _metadata = EntityProperties.Settings.Aggregate(new Dictionary<Uri, IMetadataElement>(), Process);
        }
        
        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }

        private Dictionary<Uri, IMetadataElement> Process(Dictionary<Uri, IMetadataElement> metadata,
                                                          KeyValuePair<IEntityType, IEnumerable<EntityPropertyMetadata>> entityInfo)
        {
            var targetEntity = entityInfo.Key;

            HierarchyMetadata entityMetadata =
                HierarchyMetadata
                    .Config
                    .Id.Is(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataEntitiesIdentity>(entityInfo.Key.Description))
                    .WithFeatures(new RelatedEntityFeature(targetEntity))
                    .Childs(entityInfo.Value.Cast<IMetadataElement>().ToArray());

            metadata.Add(entityMetadata.Identity.Id, entityMetadata);

            return metadata;
        }
    }
}
