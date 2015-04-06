using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.OrderPositions
{
    public interface IOrderReplaceOrderPositionAdvertisementLinksAggregateService : IAggregateSpecificOperation<Order, ReplaceOrderPositionAdvertisementLinksIdentity>
    {
        void Replace(IEnumerable<OrderPositionAdvertisement> replacedLinks, IEnumerable<AdvertisementLinkDescriptor> newLinks);
    }

    public sealed class AdvertisementLinkDescriptor
    {
        public long OrderPositionId { get; set; }
        public long? AdvertisementId { get; set; }
        public long? CategoryId { get; set; }
        public long? FirmAddressId { get; set; }
        public long PositionId { get; set; }
        public long? ThemeId { get; set; }
    }
}
