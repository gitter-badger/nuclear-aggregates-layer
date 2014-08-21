using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel
{
    public interface IAdvertisementReadModel : IAggregateReadModel<Advertisement>
    {
        [Obsolete]
        void Convert(OrderPositionInfo orderPositionInfo);

        AdvertisementElementModifyDto GetAdvertisementInfoForElement(long advertisementElementId);
        long[] GetDependedOrderIds(IEnumerable<long> advertisementIds);
        long[] GetDependedOrderIdsByAdvertisementElements(IEnumerable<long> advertisementElementIds);
        AdvertisementMailNotificationDto GetMailNotificationDto(long advertisementElementId);
        AdvertisementElementStatus GetAdvertisementElementStatus(long advertisementElementId);
        IEnumerable<AdvertisementElementCreationDto> GetElementsToCreate(long advertisementTemplateId);
        Advertisement GetAdvertisement(long advertisementId);
        IEnumerable<long> GetElementDenialReasonIds(long advertisementElementId);
        AdvertisementElementDenialReason GetAdvertisementElementDenialReason(long advertisementElementDenialReasonId);
        AdvertisementElementValidationState GetAdvertisementElementValidationState(long advertisementElementId);
    }
}