using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 04.12.2013}: 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface IGetOrderRequestMessagesOperationService : IOperation<GetOrderRequestMessagesIdentity>
    {
        IEnumerable<RequestMessageDetailDto> GetRequestMessages(long requestId);
    }
}
