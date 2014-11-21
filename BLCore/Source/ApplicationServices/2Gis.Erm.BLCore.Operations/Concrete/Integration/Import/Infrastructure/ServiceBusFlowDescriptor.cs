using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure
{
    public class ServiceBusFlowDescriptor
    {
        private readonly string _flowName;
        private readonly Type _flowType;
        private readonly IEnumerable<ServiceBusObjectDescriptor> _objects;

        public ServiceBusFlowDescriptor(string flowName, Type flowType, IEnumerable<ServiceBusObjectDescriptor> objects)
        {
            _flowName = flowName;
            _flowType = flowType;
            _objects = objects;
        }

        public string FlowName
        {
            get { return _flowName; }
        }

        public Type FlowType
        {
            get { return _flowType; }
        }

        public IEnumerable<ServiceBusObjectDescriptor> Objects
        {
            get { return _objects; }
        }
    }
}