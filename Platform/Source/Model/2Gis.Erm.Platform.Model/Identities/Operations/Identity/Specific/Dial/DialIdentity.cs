using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Dial
{
    [DataContract]
    public sealed class DialIdentity : OperationIdentityBase<DialIdentity>
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DialIndentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Dial";
            }
        }
    }
}
