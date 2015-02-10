using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public sealed class BillsConsistencyException : BusinessLogicException
    {
        public BillsConsistencyException(string message) 
            : base(message)
        {
        }

        protected BillsConsistencyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
