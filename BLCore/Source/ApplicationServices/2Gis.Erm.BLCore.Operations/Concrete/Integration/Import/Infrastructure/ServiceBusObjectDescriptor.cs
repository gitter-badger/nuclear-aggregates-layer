using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure
{
    public class ServiceBusObjectDescriptor
    {
        private readonly Type _dtoType;
        private readonly string _objectTypeName;
        private readonly ServiceBusObjectProcessingOrder _processingOrder;

        public ServiceBusObjectDescriptor(string objectTypeName, Type dtoType, ServiceBusObjectProcessingOrder processingOrder)
        {
            _objectTypeName = objectTypeName;
            _dtoType = dtoType;
            _processingOrder = processingOrder;
        }

        public string ObjectTypeName
        {
            get { return _objectTypeName; }
        }

        public Type DtoType
        {
            get { return _dtoType; }
        }

        public ServiceBusObjectProcessingOrder ProcessingOrder
        {
            get { return _processingOrder; }
        }
    }
}