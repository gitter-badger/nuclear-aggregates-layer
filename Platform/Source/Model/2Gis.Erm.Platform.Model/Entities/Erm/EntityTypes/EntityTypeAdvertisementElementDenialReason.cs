using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAdvertisementElementDenialReason : EntityTypeBase<EntityTypeAdvertisementElementDenialReason>
    {
        public override int Id
        {
            get { return EntityTypeIds.AdvertisementElementDenialReason; }
        }

        public override string Description
        {
            get { return "AdvertisementElementDenialReason"; }
        }
    }
}