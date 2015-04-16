using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition
{
    [DataContract]
    public sealed class CopyDeniedPositionsIdentity : OperationIdentityBase<CopyDeniedPositionsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CopyDeniedPositionsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Copy denied positions";
            }
        }
    }
}
