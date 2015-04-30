using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel
{
    public static partial class AdvertisementSpecs
    {
        public static class AdvertisementElementStatuses
        {
            public static class Find
            {
                public static FindSpecification<AdvertisementElementStatus> ByAdvertisementElement(long advertisementElementId)
                {
                    return new FindSpecification<AdvertisementElementStatus>(x => x.Id == advertisementElementId);
                }
            }
        }
    }
}