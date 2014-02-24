using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Qds
{
    [Serializable]
    public class DocsStorageException: Exception
    {
        public DocsStorageException()
        {
        }

        public DocsStorageException(string message) : base(message)
        {
        }

        public DocsStorageException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DocsStorageException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}