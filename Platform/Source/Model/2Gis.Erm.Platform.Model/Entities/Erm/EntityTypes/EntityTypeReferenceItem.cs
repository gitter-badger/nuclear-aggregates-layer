using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeReferenceItem : EntityTypeBase<EntityTypeReferenceItem>
    {
        public override int Id
        {
            get { return EntityTypeIds.ReferenceItem; }
        }

        public override string Description
        {
            get { return "ReferenceItem"; }
        }
    }
}