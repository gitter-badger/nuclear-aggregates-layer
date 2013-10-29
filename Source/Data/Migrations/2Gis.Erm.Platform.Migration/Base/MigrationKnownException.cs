using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class MigrationKnownException : Exception
    {
        public MigrationKnownException()
        {
        }

        public MigrationKnownException(string message) : base(message)
        {
        }

        protected MigrationKnownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MigrationKnownException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
