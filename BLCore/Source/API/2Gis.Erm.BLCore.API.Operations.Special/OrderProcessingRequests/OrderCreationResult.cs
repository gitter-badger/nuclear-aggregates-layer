using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public class OrderCreationResult
    {
        public OrderDto Order { get; set; }
        public IEnumerable<IMessageWithType> Messages { get; set; }

        public class OrderDto
        {
            public long Id { get; set; }
            public string Number { get; set; }
        }
    }
}