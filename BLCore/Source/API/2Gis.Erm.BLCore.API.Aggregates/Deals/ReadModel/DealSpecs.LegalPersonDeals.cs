using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel
{
    public static partial class DealSpecs
    {
        public static class LegalPersonDeals
        {
            public static class Find
            {
                public static FindSpecification<LegalPersonDeal> ByDealAndLegalPersonIds(long dealId, long legalPersonId)
                {
                    return new FindSpecification<LegalPersonDeal>(x => x.DealId == dealId && x.LegalPersonId == legalPersonId);
                }

                public static FindSpecification<LegalPersonDeal> ByDeal(long dealId)
                {
                    return new FindSpecification<LegalPersonDeal>(x => x.DealId == dealId);
                }

                public static FindSpecification<LegalPersonDeal> Main()
                {
                    return new FindSpecification<LegalPersonDeal>(x => x.IsMain);
                }
            }
        }
    }
}