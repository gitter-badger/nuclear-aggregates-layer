using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 28.11.2013}: IReadOnlyModel
    // TODO {d.ivanov, 28.11.2013}: ляжет в 2Gis.Erm.BLCore.Aggregates\OrderProcessingRequestMessages\ReadModel\IReadOrderRequestMessagesService
    public interface IReadOrderRequestMessagesService : ISimplifiedModelConsumer
    {
        IEnumerable<RequestMessageDetailDto> GetRequestMessages(long requestId);
    }
}
