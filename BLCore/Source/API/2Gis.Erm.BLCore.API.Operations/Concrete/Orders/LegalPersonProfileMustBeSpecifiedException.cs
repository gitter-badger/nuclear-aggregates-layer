using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class LegalPersonProfileMustBeSpecifiedException : FieldNotSpecifiedException
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
