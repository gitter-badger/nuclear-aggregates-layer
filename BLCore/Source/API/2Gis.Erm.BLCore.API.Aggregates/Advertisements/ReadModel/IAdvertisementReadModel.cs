using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel
{
    public interface IAdvertisementReadModel : IAggregateReadModel<Advertisement>
    {
        AdvertisementElementModifyDto GetAdvertisementInfoForElement(long advertisementElementId);
        IReadOnlyCollection<long> GetDependedOrderIds(IEnumerable<long> advertisementIds);
        IReadOnlyCollection<long> GetDependedOrderIdsByAdvertisementElements(IEnumerable<long> advertisementElementIds);
        AdvertisementMailNotificationDto GetMailNotificationDto(long advertisementElementId);
        AdvertisementElementStatus GetAdvertisementElementStatus(long advertisementElementId);
        IEnumerable<AdvertisementElementCreationDto> GetElementsToCreate(long advertisementTemplateId);
        Advertisement GetAdvertisement(long advertisementId);
        IEnumerable<long> GetElementDenialReasonIds(long advertisementElementId);
        AdvertisementElementDenialReason GetAdvertisementElementDenialReason(long advertisementElementDenialReasonId);
        AdvertisementElementValidationState GetAdvertisementElementValidationState(long advertisementElementId);
    }
}