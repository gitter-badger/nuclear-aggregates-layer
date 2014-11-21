using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies
{
    public sealed class TopologyProcessingResults
    {
        public IReadOnlyCollection<IMessage> Succeeded { get; set; }
        public IReadOnlyCollection<IMessage> Failed { get; set; }
    }
}