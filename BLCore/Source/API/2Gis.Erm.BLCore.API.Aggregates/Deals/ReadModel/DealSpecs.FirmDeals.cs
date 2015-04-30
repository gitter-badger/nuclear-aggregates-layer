using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel
{
    public static partial class DealSpecs
    {
        public static class FirmDeals
        {
            public static class Find
            {
                public static FindSpecification<FirmDeal> ByDealAndFirmIds(long dealId, long firmId)
                {
                    return new FindSpecification<FirmDeal>(x => x.DealId == dealId && x.FirmId == firmId);
                }

                public static FindSpecification<FirmDeal> ByDeal(long dealId)
                {
                    return new FindSpecification<FirmDeal>(x => x.DealId == dealId);
                }
            }
        }
    }
}