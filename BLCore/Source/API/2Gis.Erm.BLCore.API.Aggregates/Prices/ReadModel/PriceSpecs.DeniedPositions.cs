using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel
{
    public static partial class PriceSpecs
    {
        public static class DeniedPositions
        {
            public static class Find
            {
                public static FindSpecification<DeniedPosition> ByPrice(long priceId)
                {
                    return new FindSpecification<DeniedPosition>(x => x.PriceId == priceId);
                }

                public static FindSpecification<DeniedPosition> ByPositions(long positionId, long positionDeniedId)
                {
                    return new FindSpecification<DeniedPosition>(x => x.PositionId == positionId && x.PositionDeniedId == positionDeniedId);
                }
            }
        }
    }
}