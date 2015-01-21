using System;
using System.Runtime.Serialization;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class OperationException<TEntity, TOperation> : BusinessLogicException
        where TEntity : IEntityKey
        where TOperation : IOperationIdentity
    {
        public OperationException()
        {
        }

        public OperationException(string message)
            : base(message)
        {
        }

        public OperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OperationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}