using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.ReadModel
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

        public AdvertisementElementModifyDto GetAdvertisementInfoForElement(long advertisementElementId)
        {
            return _finder.Find(Specs.Find.ById<AdvertisementElement>(advertisementElementId))
                                .Select(x => new AdvertisementElementModifyDto
                                {
                                    IsDummy = x.Advertisement.FirmId == null,
                                    IsPublished = x.Advertisement.AdvertisementTemplate.IsPublished,
                                    AdvertisementId = x.Advertisement.Id,
                                    Element = x,
                                    ElementTemplate = x.AdvertisementElementTemplate,
                                    PreviousStatusElementStatus = (AdvertisementElementStatus)x.Status,
                                    ClonedDummies = x.AdvertisementElementTemplate
                                                            .AdvertisementElements
                                                                .Where(ae => ae.Id != advertisementElementId 
                                                                            && !ae.IsDeleted 
                                                                            && ae.Advertisement.FirmId == null
                                                                            && !ae.Advertisement.AdvertisementTemplate.IsPublished)
                                                                .Distinct()
                                })
                                .Single();
        }

        public AdvertisementMailNotificationDto GetMailNotificationDto(long advertisementElementId)
        {
            return _finder.Find(Specs.Find.ById<AdvertisementElement>(advertisementElementId))
                        .Select(x => new AdvertisementMailNotificationDto
                        {
                            FirmRef = new EntityReference { Id = x.Advertisement.Firm.Id, Name = x.Advertisement.Firm.Name },
                            FirmOwnerCode = x.Advertisement.Firm.OwnerCode,
                            AdvertisementRef = new EntityReference { Id = x.Advertisement.Id, Name = x.Advertisement.Name },
                            AdvertisementTemplateName = x.Advertisement.AdvertisementTemplate.Name,
                            AdvertisementElementTemplateName = x.AdvertisementElementTemplate.Name,
                            OrderRefs = x.Advertisement.OrderPositionAdvertisements
                                        .Where(opa => opa.OrderPosition.IsActive && !opa.OrderPosition.IsDeleted
                                                            && opa.OrderPosition.Order.IsActive && !opa.OrderPosition.Order.IsDeleted)
                                        .Select(opa => new EntityReference { Id = opa.OrderPosition.Order.Id, Name = opa.OrderPosition.Order.Number })
                                        .Distinct()
                        })
                        .Single();
        }

        public long[] GetDependedOrderIds(IEnumerable<long> advertisementIds)
        {
            var orderIds = _finder.Find<Advertisement>(x => advertisementIds.Contains(x.Id))
                                .SelectMany(x => x.OrderPositionAdvertisements)
                .Select(x => x.OrderPosition)
                .Where(x => !x.IsDeleted && x.IsActive)
                .Select(x => x.Order)
                .Where(x => !x.IsDeleted && x.IsActive)
                .Select(x => x.Id)
                .Distinct()
                .ToArray();

            return orderIds;
        }
    }
}