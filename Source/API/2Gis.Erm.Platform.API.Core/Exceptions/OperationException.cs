using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

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