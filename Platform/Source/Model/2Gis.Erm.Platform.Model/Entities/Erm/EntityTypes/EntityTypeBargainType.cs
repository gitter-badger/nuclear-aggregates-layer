using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBargainType : EntityTypeBase<EntityTypeBargainType>
    {
        public override int Id
        {
            get { return EntityTypeIds.BargainType; }
        }

        public override string Description
        {
            get { return "BargainType"; }
        }
    }
}