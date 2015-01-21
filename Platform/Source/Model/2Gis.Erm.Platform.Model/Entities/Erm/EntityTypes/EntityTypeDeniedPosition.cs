using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeDeniedPosition : EntityTypeBase<EntityTypeDeniedPosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.DeniedPosition; }
        }

        public override string Description
        {
            get { return "DeniedPosition"; }
        }
    }
}