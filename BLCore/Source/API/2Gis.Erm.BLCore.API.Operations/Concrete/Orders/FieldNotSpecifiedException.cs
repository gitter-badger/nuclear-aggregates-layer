using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public class FieldNotSpecifiedException : BusinessLogicException
    {
        public FieldNotSpecifiedException(string message)
            : base(message)
        {
        }

        protected FieldNotSpecifiedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
