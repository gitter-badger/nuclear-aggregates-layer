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

        AdvertisementMailNotificationDto GetMailNotificationDto(long advertisementElementId);

        Firm GetFirmByAdvertisementElement(long advertisementElementId);
    }
}