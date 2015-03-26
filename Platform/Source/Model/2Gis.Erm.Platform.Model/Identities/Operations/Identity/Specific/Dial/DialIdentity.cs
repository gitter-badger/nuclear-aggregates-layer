using System.Runtime.Serialization;

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
