using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public sealed class NullOrderProcessingRequestEmailSender : IOrderProcessingRequestEmailSender
    {
        public OrderProcessingRequestEmailSendResult SendProcessingMessages(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest, IEnumerable<IMessageWithType> messagesToSend)
        {
            return new OrderProcessingRequestEmailSendResult();
        }
    }
}