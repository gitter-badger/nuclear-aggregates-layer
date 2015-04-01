using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    internal class UnsupportedExportOperationException : BusinessLogicException
    {
        public UnsupportedExportOperationException(PerformedBusinessOperation operation)
            : base(string.Format("Operation {0} with id {1} is not supported for export", operation.Operation, operation.Id))
        {
        }

        protected UnsupportedExportOperationException(SerializationInfo serializationInfo, StreamingContext streamingContext) 
            : base(serializationInfo, streamingContext)
        {
        }
    }
}