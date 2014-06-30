using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    [Serializable]
    public class TrackerStateException : Exception
    {
        public TrackerStateException()
        {
        }

        public TrackerStateException(string message) : base(message)
        {
        }

        public TrackerStateException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TrackerStateException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}