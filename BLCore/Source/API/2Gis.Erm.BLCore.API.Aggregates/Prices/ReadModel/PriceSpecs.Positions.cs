using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel
{
    public static partial class PriceSpecs
    {
        public static class Positions
        {
            public static class Find
            {
                public static FindSpecification<Position> WithSortingSpecified()
                {
                    return new FindSpecification<Position>(x => x.SortingIndex.HasValue);
                }

                public static FindSpecification<PositionChildren> UsedAsChildElement(long positionId)
                {
                    return new FindSpecification<PositionChildren>(x => !x.IsDeleted && x.ChildPositionId == positionId);
                }

                public static FindSpecification<Position> ByPricePosition(long pricePositionId)
                {
                    return new FindSpecification<Position>(x => x.PricePositions.Any(y => y.Id == pricePositionId));
                }
            }

            public class Select
            {
                public static ISelectSpecification<Position, PositionSortingOrderDto> PositionSortingOrderDto()
                {
                    return new SelectSpecification<Position, PositionSortingOrderDto>(
                        position => new PositionSortingOrderDto
                                        {
                                            Name = position.Name,
                                            Id = position.Id,
                                            Index = position.SortingIndex
                                        });
                }
            }
        }
    }
}