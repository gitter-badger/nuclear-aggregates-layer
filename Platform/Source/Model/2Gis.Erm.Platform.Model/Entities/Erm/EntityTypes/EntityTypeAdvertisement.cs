using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdvertisement : EntityTypeBase<EntityTypeAdvertisement>
    {
        public override int Id
        {
            get { return EntityTypeIds.Advertisement; }
        }

        public override string Description
        {
            get { return "Advertisement"; }
        }
    }
}