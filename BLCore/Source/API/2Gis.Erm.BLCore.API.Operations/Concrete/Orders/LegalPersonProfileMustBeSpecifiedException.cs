using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class LegalPersonProfileMustBeSpecifiedException : BusinessLogicException
    {
        public LegalPersonProfileMustBeSpecifiedException()
            : base(BLResources.LegalPersonProfileMustBeSpecified)
        {
        }

        protected LegalPersonProfileMustBeSpecifiedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
