using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.ReadModel
{
    public sealed class AdvertisementReadModel : IAdvertisementReadModel
    {
        private readonly IFinder _finder;

        public AdvertisementReadModel(IFinder finder)
        {
            _finder = finder;
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
                                               AdvertisementRef = new EntityReference { Id = x.Advertisement.Id, Name = x.Advertisement.Name },
                                               AdvertisementTemplateName = x.Advertisement.AdvertisementTemplate.Name,
                                               AdvertisementElementTemplateName = x.AdvertisementElementTemplate.Name,
                                               OrderRefs = x.Advertisement.OrderPositionAdvertisements
                                                            .Where(opa => opa.OrderPosition.IsActive && !opa.OrderPosition.IsDeleted
                                                                          && opa.OrderPosition.Order.IsActive && !opa.OrderPosition.Order.IsDeleted)
                                                            .Select(
                                                                    opa =>
                                                                    new AdvertisementMailNotificationDto.OrderInfo
                                                                        {
                                                                            Id = opa.OrderPosition.Order.Id,
                                                                            Number = opa.OrderPosition.Order.Number,
                                                                            OwnerCode = opa.OrderPosition.Order.OwnerCode
                                                                        })
                                                            .Distinct()
                                           })
                          .Single();
        }

        public AdvertisementElementStatus GetAdvertisementElementStatus(long advertisementElementId)
        {
            return _finder.FindOne(AdvertisementSpecs.AdvertisementElementStatuses.Find.ByAdvertisementElement(advertisementElementId));
        }

        public IEnumerable<AdvertisementElementCreationDto> GetElementsToCreate(long advertisementTemplateId)
        {
            return _finder.Find(Specs.Find.ById<AdvertisementTemplate>(advertisementTemplateId))
                          .SelectMany(x => x.AdsTemplatesAdsElementTemplates
                                            .Where(y => !y.IsDeleted)
                                            .Select(y => new AdvertisementElementCreationDto
                                                {
                                                    AdsTemplatesAdsElementTemplateId = y.Id,
                                                    AdvertisementElementTemplateId = y.AdsElementTemplateId,
                                                    IsFasComment = y.AdvertisementElementTemplate.RestrictionType == AdvertisementElementRestrictionType.FasComment,
                                                    NeedsValidation = y.AdvertisementElementTemplate.NeedsValidation,
                                                    IsRequired = y.AdvertisementElementTemplate.IsRequired,
                                                    DummyAdvertisementId = y.AdvertisementTemplate.DummyAdvertisementId
                                                }))
                          .ToArray();
        }

        public Advertisement GetAdvertisement(long advertisementId)
        {
            return _finder.FindOne(Specs.Find.ById<Advertisement>(advertisementId));
        }

        public IEnumerable<long> GetElementDenialReasonIds(long advertisementElementId)
        {
            return
                _finder.Find(AdvertisementSpecs.AdvertisementElementDenialReasons.Find.ByAdvertisementElement(advertisementElementId))
                       .Select(x => x.Id)
                       .ToArray();
        }

        public AdvertisementElementDenialReason GetAdvertisementElementDenialReason(long advertisementElementDenialReasonId)
        {
            return _finder.FindOne(Specs.Find.ById<AdvertisementElementDenialReason>(advertisementElementDenialReasonId));
        }

        public AdvertisementElementValidationState GetAdvertisementElementValidationState(long advertisementElementId)
        {
            return _finder.Find(AdvertisementSpecs.AdvertisementElementStatuses.Find.ByAdvertisementElement(advertisementElementId))
                          .Select(x => new AdvertisementElementValidationState
                              {
                                  NeedsValidation =
                                      x.AdvertisementElement.AdvertisementElementTemplate.NeedsValidation && x.AdvertisementElement.Advertisement.FirmId != null,
                                  CurrentStatus = x
                              })
                          .Single();
        }

        public IReadOnlyCollection<long> GetDependedOrderIds(IEnumerable<long> advertisementIds)
        {
            return _finder.Find(new FindSpecification<Advertisement>(x => advertisementIds.Contains(x.Id)))
                                  .SelectMany(x => x.OrderPositionAdvertisements)
                                  .Select(x => x.OrderPosition)
                                  .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                                  .Select(x => x.Order)
                                  .Where(Specs.Find.ActiveAndNotDeleted<Order>())
                                  .Select(x => x.Id)
                                  .Distinct()
                                  .ToArray();
        }

        public IReadOnlyCollection<long> GetDependedOrderIdsByAdvertisementElements(IEnumerable<long> advertisementElementIds)
        {
            return _finder.Find(Specs.Find.ByIds<AdvertisementElement>(advertisementElementIds))
                                  .SelectMany(x => x.Advertisement.OrderPositionAdvertisements)
                                  .Select(x => x.OrderPosition)
                                  .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                                  .Select(x => x.Order)
                                  .Where(Specs.Find.ActiveAndNotDeleted<Order>())
                                  .Select(x => x.Id)
                                  .Distinct()
                                  .ToArray();
        }
    }
}