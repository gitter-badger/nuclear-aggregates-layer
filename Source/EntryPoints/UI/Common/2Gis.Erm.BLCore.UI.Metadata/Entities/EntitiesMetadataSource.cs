using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;

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
                                                          KeyValuePair<EntityName, IEnumerable<EntityPropertyMetadata>> entityInfo)
        {
            var targetEntity = entityInfo.Key;

            HierarchyMetadata entityMetadata =
                HierarchyMetadata
                    .Config
                    .Id.Is(IdBuilder.For<MetadataEntitiesIdentity>(entityInfo.Key.ToString()))
                    .WithFeatures(new RelatedEntityFeature(targetEntity))
                    .Childs(entityInfo.Value.Cast<IMetadataElement>().ToArray());

            metadata.Add(entityMetadata.Identity.Id, entityMetadata);

            return metadata;
        }
    }
}
