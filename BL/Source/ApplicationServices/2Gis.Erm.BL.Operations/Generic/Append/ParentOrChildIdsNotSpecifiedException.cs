using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    [Serializable]
    public class ParentOrChildIdsNotSpecifiedException : AppendException
    {
        public ParentOrChildIdsNotSpecifiedException()
        {
        }

        public ParentOrChildIdsNotSpecifiedException(string message) : base(message)
        {
        }

        public ParentOrChildIdsNotSpecifiedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ParentOrChildIdsNotSpecifiedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}