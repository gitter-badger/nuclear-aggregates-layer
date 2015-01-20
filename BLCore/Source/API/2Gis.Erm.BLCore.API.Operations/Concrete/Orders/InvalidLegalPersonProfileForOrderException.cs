using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class InvalidLegalPersonProfileForOrderException : BusinessLogicException
    {
        public InvalidLegalPersonProfileForOrderException()
            : base(BLResources.InvalidLegalPersonProfileForOrder)
        {
        }

        protected InvalidLegalPersonProfileForOrderException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
