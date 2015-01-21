using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBargain : EntityTypeBase<EntityTypeBargain>
    {
        public override int Id
        {
            get { return EntityTypeIds.Bargain; }
        }

        public override string Description
        {
            get { return "Bargain"; }
        }
    }
}