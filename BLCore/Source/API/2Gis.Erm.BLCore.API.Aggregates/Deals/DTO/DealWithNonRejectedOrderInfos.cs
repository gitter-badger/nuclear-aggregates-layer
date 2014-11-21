using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public class DealWithNonRejectedOrderInfos
    {
        public Deal Deal { get; set; }
        public IEnumerable<OrderInfo> OrderInfos { get; set; }

        public class OrderInfo
        {
            public long Id { get; set; }
            public decimal AmountWithdrawn { get; set; }
        }
    }
}