using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    [Serializable]
    // COMMENT {f.zaharov, 01.09.2014}: Не особо корректное название с точки зрения англ языка (не придираюсь :))
    // DONE {f.zaharov, 01.09.2014}: Согласен, так лучше? Если нет, сразу дай свой вариант :)
    public class InvalidEntityTypesForLinkingException : AppendException
    {
        public InvalidEntityTypesForLinkingException()
        {
        }

        public InvalidEntityTypesForLinkingException(string message) : base(message)
        {
        }

        public InvalidEntityTypesForLinkingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidEntityTypesForLinkingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}