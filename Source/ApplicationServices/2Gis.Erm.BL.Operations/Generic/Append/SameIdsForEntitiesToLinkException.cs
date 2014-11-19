using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    [Serializable]
    public class SameIdsForEntitiesToLinkException : Exception
    {
        public SameIdsForEntitiesToLinkException()
        {
        }

        public SameIdsForEntitiesToLinkException(string message) : base(message)
        {
        }

        public SameIdsForEntitiesToLinkException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SameIdsForEntitiesToLinkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}