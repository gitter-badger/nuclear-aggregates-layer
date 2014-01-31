using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements
{
    public sealed class AdvertisementReadModel : IAdvertisementReadModel
    {
        private readonly IFinder _finder;

        public AdvertisementReadModel(IFinder finder)
        {
            _finder = finder;
        }

        [Obsolete]
        // FIXME {all, 30.01.2014}: поддержка legacy рекламных материлов в "старом формате" (из ДГПП), подробнее см. тип OldFormatAdvertisementMaterialDetector
        public void Convert(OrderPositionInfo orderPositionInfo)
        {
            const int PriorityExportCode = 1;
            const int BannerExportCode = 8;

            if (orderPositionInfo.ProductType != PriorityExportCode && orderPositionInfo.ProductType != BannerExportCode)
            {
                return;
            }

            foreach (var advertisingMaterialInfo in orderPositionInfo.AdvertisingMaterials)
            {
                if (advertisingMaterialInfo.StableRubrIds.Any())
                {
                    continue;
                }

                var categoryDgppIds =
                    (from orderPosition in _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionInfo.Id))
                     from firmAddress in orderPosition.Order.Firm.FirmAddresses
                     from categoryFirmAddress in firmAddress.CategoryFirmAddresses
                     where categoryFirmAddress.IsActive
                           && !categoryFirmAddress.IsDeleted
                           && firmAddress.IsActive
                           && !firmAddress.IsDeleted
                     select categoryFirmAddress.Category.Id)
                        .Distinct()
                        .ToArray();

                advertisingMaterialInfo.StableRubrIds = categoryDgppIds;
            }
        }
    }
}