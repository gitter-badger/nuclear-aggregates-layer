using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    [Serializable]
    public class ClientLinkAlreadyExistsException : Exception
    {
        public ClientLinkAlreadyExistsException()
        {
        }

        public ClientLinkAlreadyExistsException(string message) : base(message)
        {
        }

        public ClientLinkAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ClientLinkAlreadyExistsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}