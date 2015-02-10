using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class PositionSpecs
    {
        public static class Find
        {
            public static FindSpecification<PositionChildren> UsedAsChildElement(long positionId)
            {
                return new FindSpecification<PositionChildren>(x => !x.IsDeleted && x.ChildPositionId == positionId);
            }

            public static FindSpecification<Position> ByPricePosition(long pricePositionId)
            {
                return new FindSpecification<Position>(x => x.PricePositions.Any(y => y.Id == pricePositionId));
            }
        }
    }
}