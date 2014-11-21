using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure
{
    public class ImportMetadataProvider : IImportMetadataProvider
    {
        private readonly IEnumerable<ServiceBusFlowDescriptor> _flowDescriptors;

        public ImportMetadataProvider(IEnumerable<ServiceBusFlowDescriptor> flowDescriptors)
        {
            _flowDescriptors = flowDescriptors;
        }

        public Type GetObjectType(string flowName, string busObjectTypeName)
        {
            var flow = _flowDescriptors.Single(x => x.FlowName == flowName);
            return flow.Objects.Where(x => x.ObjectTypeName.EqualsIgnoreCase(busObjectTypeName)).Select(x => x.DtoType).Single();
        }

        public bool IsSupported(string flowName)
        {
            return _flowDescriptors.Any(x => x.FlowName.EqualsIgnoreCase(flowName));
        }

        public bool IsSupported(string flowName, string busObjectTypeName)
        {
            var flow = _flowDescriptors.SingleOrDefault(x => x.FlowName.EqualsIgnoreCase(flowName));
            if (flow == null)
            {
                return false;
            }

            return flow.Objects.Any(x => x.ObjectTypeName.EqualsIgnoreCase(busObjectTypeName));
        }

        public int GetProcessingOrder(string flowName, string busObjectTypeName)
        {
            var flow = _flowDescriptors.Single(x => x.FlowName.EqualsIgnoreCase(flowName));
            return flow.Objects.Where(x => x.ObjectTypeName.EqualsIgnoreCase(busObjectTypeName)).Select(x => (int)x.ProcessingOrder).Single();
        }
    }
}