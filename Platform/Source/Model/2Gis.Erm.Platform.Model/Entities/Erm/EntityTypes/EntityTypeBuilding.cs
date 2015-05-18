using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBuilding : EntityTypeBase<EntityTypeBuilding>
    {
        public override int Id
        {
            get { return EntityTypeIds.Building; }
        }

        public override string Description
        {
            get { return "Building"; }
        }
    }
}