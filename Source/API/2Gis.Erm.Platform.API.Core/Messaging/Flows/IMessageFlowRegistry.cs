using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Flows
{
    public interface IMessageFlowRegistry
    {
        IEnumerable<IMessageFlow> Flows { get; }
        IEnumerable<IMessageFlow> GetChildFlows(IMessageFlow parentFlow);
        bool TryResolve(string flowDescriptor, out IMessageFlow messageFlow);
    }
}