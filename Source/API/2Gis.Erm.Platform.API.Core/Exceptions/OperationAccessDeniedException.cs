using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class OperationAccessDeniedException : BusinessLogicException
    {
        public OperationAccessDeniedException(string message) :
            base(message)
        {
        }

        public OperationAccessDeniedException(IOperationIdentity operation) :
            base(GenerateMessage(operation))
        {
        }

        protected OperationAccessDeniedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GenerateMessage(IOperationIdentity operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            return string.Format("Доступ на выполнение операции '{0}' запрещен", operation.Description);
        }
    }
}