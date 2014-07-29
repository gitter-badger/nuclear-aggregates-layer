using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public class OrderProcessingRequestEmailSendResult
    {
        public OrderProcessingRequestEmailSendResult()
        {
            Errors = new List<string>();
        }

        public ICollection<string> Errors { get; private set; }
    }
}