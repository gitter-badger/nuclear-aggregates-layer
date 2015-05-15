using System;
using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class OperationAccessDeniedException : BusinessLogicException
    {
        public OperationAccessDeniedException(string message) :
            base(message)
        {
        }

        public OperationAccessDeniedException(string message, Exception innerException)
            : base(message, innerException)
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

            return string.Format(ResPlatform.AccessToOperationIsDenied, operation.Description);
        }
    }
}