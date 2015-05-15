using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdvertisementElementStatus : EntityTypeBase<EntityTypeAdvertisementElementStatus>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdvertisementElementStatus; }
        }

        public override string Description
        {
            get { return "AdvertisementElementStatus"; }
        }
    }
}