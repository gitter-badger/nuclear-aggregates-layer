using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies
{
    public interface IMessageProcessingTopology
    {
        Task<TopologyProcessingResults> ProcessAsync(IReadOnlyList<IMessage> messages);
    }
}
