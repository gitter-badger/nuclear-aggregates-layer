using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations.Special\OrderProcessingRequest
    public class NullOrderProcessingRequestEmailSender : IOrderProcessingRequestEmailSender
    {
        public OrderProcessingRequestEmailSendResult SendProcessingMessages(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, IEnumerable<IMessageWithType> messagesToSend)
        {
            return new OrderProcessingRequestEmailSendResult();
        }
    }
}