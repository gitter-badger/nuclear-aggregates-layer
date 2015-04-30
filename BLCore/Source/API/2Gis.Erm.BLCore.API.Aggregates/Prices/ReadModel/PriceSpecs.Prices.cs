using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel
{
    public static partial class PriceSpecs
    {
        public static class Prices
        {
            public static class Find
            {
                public static FindSpecification<Price> Linked()
                {
                    return new FindSpecification<Price>(x => x.PricePositions
                                                              .Any(pp => !pp.IsDeleted &&
                                                                         pp.OrderPositions.Any(op => !op.IsDeleted && op.Order.IsActive && !op.Order.IsDeleted)));
                }
            }

            public static class Select
            {
                public static ISelectSpecification<Price, PriceValidationDto> PriceValidationDto()
                {
                    return new SelectSpecification<Price, PriceValidationDto>(x => new PriceValidationDto
                        {
                            IsPriceDeleted = x.IsDeleted,
                            IsDeniedPositionsNotValid = x.DeniedPositions
                                                         .Where(y => !y.IsDeleted)
                                                         .Select(y => y.PositionDenied)
                                                         .Distinct()
                                                         .Any(y => !y.IsActive || y.IsDeleted),
                            IsPricePositionsNotValid = x.PricePositions
                                                        .Where(y => y.IsActive && !y.IsDeleted)
                                                        .Select(y => y.Position)
                                                        .Distinct()
                                                        .Any(y => !y.IsActive || y.IsDeleted),
                            IsAssociatedPositionsNotValid = x.PricePositions
                                                             .Where(y => y.IsActive && !y.IsDeleted)
                                                             .SelectMany(y => y.AssociatedPositionsGroups)
                                                             .Distinct()
                                                             .SelectMany(y => y.AssociatedPositions)
                                                             .Distinct()
                                                             .Select(y => y.Position)
                                                             .Any(y => !y.IsActive || y.IsDeleted),
                        });
                }
            }
        }
    }
}