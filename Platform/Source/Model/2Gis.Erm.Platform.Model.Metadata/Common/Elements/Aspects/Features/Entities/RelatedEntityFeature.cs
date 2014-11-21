using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities
{
    public sealed class RelatedEntityFeature : IUniqueMetadataFeature
    {
        private readonly EntityName _entity;

        public RelatedEntityFeature(EntityName entity)
        {
            _entity = entity;
        }

        public EntityName Entity
        {
            get { return _entity; }
        }
    }
}
