using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderPositionAdvertisement : EntityTypeBase<EntityTypeOrderPositionAdvertisement>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderPositionAdvertisement; }
        }

        public override string Description
        {
            get { return "OrderPositionAdvertisement"; }
        }
    }
}