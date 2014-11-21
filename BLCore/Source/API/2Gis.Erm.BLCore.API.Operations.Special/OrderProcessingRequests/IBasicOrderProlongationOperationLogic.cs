using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface IBasicOrderProlongationOperationLogic
    {
        OrderProcessingResult ExecuteRequest(OrderProcessingRequest orderProcessingRequest);
    }
}