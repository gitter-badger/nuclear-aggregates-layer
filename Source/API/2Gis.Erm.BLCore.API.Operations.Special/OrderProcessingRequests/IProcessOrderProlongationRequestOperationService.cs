using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface IProcessOrderProlongationRequestSingleOperation : IOperation<ProlongateOrderByRequestIdentity>
    {
        OrderProcessingResult ProcessSingle(long id);
    }
}
