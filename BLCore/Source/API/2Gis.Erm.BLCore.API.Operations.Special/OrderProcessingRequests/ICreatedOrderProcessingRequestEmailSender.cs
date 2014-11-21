using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 17.12.2013}: 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface ICreatedOrderProcessingRequestEmailSender
    {
        OrderProcessingRequestEmailSendResult SendRequestIsCreatedMessage(OrderProcessingRequest orderProcessingRequest);
    }
}
