using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel
{
    public static partial class AdvertisementSpecs
    {
        public static class AdvertisementElementDenialReasons
        {
            public static class Find
            {
                public static FindSpecification<AdvertisementElementDenialReason> ByAdvertisementElement(long advertisementElementId)
                {
                    return new FindSpecification<AdvertisementElementDenialReason>(x => x.AdvertisementElementId == advertisementElementId);
                }
            }
        }
    }
}