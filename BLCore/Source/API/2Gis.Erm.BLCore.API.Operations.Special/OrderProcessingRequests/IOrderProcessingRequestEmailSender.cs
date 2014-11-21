using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface IOrderProcessingRequestEmailSender
    {
        OrderProcessingRequestEmailSendResult SendProcessingMessages(OrderProcessingRequest orderProcessingRequest,
                                    IEnumerable<IMessageWithType> messagesToSend);
    }
}
