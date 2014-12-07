using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class OrderPositions
        {
            public static class Find
            {
                public static FindSpecification<OrderPosition> ByOrder(long orderId)
                {
                    return new FindSpecification<OrderPosition>(x => x.OrderId == orderId);
                }

                public static FindSpecification<OrderPosition> PlannedProvision()
                {
                    return new FindSpecification<OrderPosition>(x => x.PricePosition.Position.AccountingMethodEnum == PositionAccountingMethod.PlannedProvision);
                } 
            }

            public static class Select
            {
                public static ISelectSpecification<OrderPosition, RepairOutdatedOrderPositionDto> RepairOutdatedOrderPositions()
                {
                    return new SelectSpecification<OrderPosition, RepairOutdatedOrderPositionDto>(
                        x => new RepairOutdatedOrderPositionDto
                            {
                                OrderPosition = x,
                                PricePosition = x.PricePosition,
                                Advertisements = x.OrderPositionAdvertisements,
                                ClonedAdvertisements = x.OrderPositionAdvertisements
                                                        .Select(adv => new AdvertisementDescriptor
                                                            {
                                                                AdvertisementId = adv.AdvertisementId,
                                                                CategoryId = adv.CategoryId,
                                                                ThemeId = adv.ThemeId,
                                                                FirmAddressId = adv.FirmAddressId,
                                                                PositionId = adv.PositionId,
                                                                IsAdvertisementRequired =
                                                                    adv.Position.AdvertisementTemplate != null &&
                                                                    adv.Position.AdvertisementTemplate.IsAdvertisementRequired
                                                            }),
                                Withdrawals = x.ReleasesWithdrawals
                            });
                }
            }
        }
    }
}