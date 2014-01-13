using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public class OrderProcessingResult
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public IEnumerable<RepairOutdatedPositionsOperationMessage> Messages { get; set; }
    }
}
