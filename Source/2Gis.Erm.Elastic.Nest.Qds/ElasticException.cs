using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    [Serializable]
    public class ElasticException: Exception
    {
        public ElasticException()
        {
        }

        public ElasticException(string message) : base(message)
        {
        }

        public ElasticException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ElasticException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}