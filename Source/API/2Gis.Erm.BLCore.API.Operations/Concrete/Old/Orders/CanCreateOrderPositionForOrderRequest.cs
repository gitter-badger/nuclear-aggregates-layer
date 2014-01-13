using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class CanCreateOrderPositionForOrderRequest : Request
    {
        public long OrderId { get; set; }
        public OrderType OrderType { get; set; }
        public long? FirmId { get; set; }
        public IEnumerable<long> OrderPositionCategoryIds { get; set; }
        public IEnumerable<long> OrderPositionFirmAddressIds { get; set; }
        public bool IsPositionComposite { get; set; }
        public int AdvertisementLinksCount { get; set; }
    }
}
