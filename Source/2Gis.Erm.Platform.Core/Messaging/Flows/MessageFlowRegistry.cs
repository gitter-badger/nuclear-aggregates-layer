using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;

namespace DoubleGis.Erm.Platform.Core.Messaging.Flows
{
    public sealed class MessageFlowRegistry : IMessageFlowRegistry
    {
        private readonly IReadOnlyDictionary<IMessageFlow, IMessageFlow[]> _flowsMap = new Dictionary<IMessageFlow, IMessageFlow[]>
            {
                { AllPerformedOperationsFlow.Instance, new IMessageFlow[] { FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance } },
                { PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance, new IMessageFlow[] { FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance } },
                { FinalStorageReplicate2MsCRMPerformedOperationsFlow.Instance, new IMessageFlow[] { FinalReplicate2MsCRMPerformedOperationsFlow.Instance } },
                { FinalReplicate2MsCRMPerformedOperationsFlow.Instance, new IMessageFlow[0] },
                { PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance, new IMessageFlow[] { ElasticRuntimeFlow.Instance } },
            };

        public IEnumerable<IMessageFlow> Flows
        {
            get { return _flowsMap.Keys; }
        }

        public IEnumerable<IMessageFlow> GetChildFlows(IMessageFlow parentFlow)
        {
            IMessageFlow[] flows;
            return !_flowsMap.TryGetValue(parentFlow, out flows) && flows == null ? Enumerable.Empty<IMessageFlow>() : flows;
        }

        public bool TryResolve(string flowDescriptor, out IMessageFlow messageFlow)
        {
            messageFlow = _flowsMap.Keys.SingleOrDefault(flow => string.Compare(flow.GetType().Name, flowDescriptor, System.StringComparison.OrdinalIgnoreCase) == 0);
            return messageFlow != null;
        }
    }
}