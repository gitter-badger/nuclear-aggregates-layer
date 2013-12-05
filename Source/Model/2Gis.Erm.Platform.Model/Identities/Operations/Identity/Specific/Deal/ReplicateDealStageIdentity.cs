using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal
{
    [DataContract]
    public sealed class ReplicateDealStageIdentity : OperationIdentityBase<ReplicateDealStageIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ReplicateDealStageIdentity; }
        }

        public override string Description
        {
            get { return "Replicate deal stage from external system (MSCRM)"; }
        }
    }
}
