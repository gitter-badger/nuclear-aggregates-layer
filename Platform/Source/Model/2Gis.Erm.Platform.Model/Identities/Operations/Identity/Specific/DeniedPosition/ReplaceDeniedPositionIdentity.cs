using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition
{
    [DataContract]
    public sealed class ReplaceDeniedPositionIdentity : OperationIdentityBase<ReplaceDeniedPositionIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ReplaceDeniedPositionIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Replace denied position";
            }
        }
    }
}
