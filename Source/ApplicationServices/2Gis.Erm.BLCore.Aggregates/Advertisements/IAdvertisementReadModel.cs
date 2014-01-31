using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements
{
    public interface IAdvertisementReadModel : IAggregateReadModel<Advertisement>
    {
        [Obsolete]
        void Convert(OrderPositionInfo orderPositionInfo);
    }
}