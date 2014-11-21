using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    [Serializable]
    public class AppendAdvertisingAgencyException : Exception
    {
        public AppendAdvertisingAgencyException()
        {
        }

        public AppendAdvertisingAgencyException(string message) : base(message)
        {
        }

        public AppendAdvertisingAgencyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AppendAdvertisingAgencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}