using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel
{
    // FIXME {a.rechkalov, 16.02.2015}: Объединить с BLCore\Source\ApplicationServices\2Gis.Erm.BLCore.Aggregates\Prices\PositionSpecs.cs
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