using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeTerritory : EntityTypeBase<EntityTypeTerritory>
    {
        public override int Id
        {
            get { return EntityTypeIds.Territory; }
        }

        public override string Description
        {
            get { return "Territory"; }
        }
    }
}