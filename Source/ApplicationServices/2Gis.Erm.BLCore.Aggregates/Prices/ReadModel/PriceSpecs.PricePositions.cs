using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel
{
    public static partial class PriceSpecs
    {
        public static class PricePositions
        {
            public static class Find
            {
                public static FindSpecification<PricePosition> ByPriceAndPosition(long priceId, long positionId)
                {
                    return new FindSpecification<PricePosition>(x => x.PriceId == priceId && x.PositionId == positionId);
                }

                public static FindSpecification<PricePosition> ByPriceAndPositionButAnother(long positionId, long priceId, long pricePositionId)
                {
                    return new FindSpecification<PricePosition>(x => x.PriceId == priceId && x.PositionId == positionId && x.Id != pricePositionId);
                }
            }
        }
    }
}