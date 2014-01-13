using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class PricePositionSpecifications
    {
        public static class Find
        {
            public static FindSpecification<PricePosition> ByPriceAndPostion(long positionId, long priceId)
            {
                return new FindSpecification<PricePosition>(x => x.PositionId == positionId && x.PriceId == priceId);
            }
        }
    }
}
