using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    // FIXME {f.zaharov, 28.08.2014}: Какой смысл в суффиксе Bl? Тип исключения выражает суть исключительной ситуации и не привязан ни к чему.
    //                                То же касается и наследников
    // DONE {f.zaharov, 29.08.2014}: done
    [Serializable]
    public class AppendException : BusinessLogicException
    {
        public AppendException()
        {
        }

        public AppendException(string message) : base(message)
        {
        }

        public AppendException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AppendException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}