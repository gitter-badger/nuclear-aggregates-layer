using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition
{
    [DataContract]
    public sealed class VerifyDeniedPositionsForDuplicatesIdentity : OperationIdentityBase<VerifyDeniedPositionsForDuplicatesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.VerifyDeniedPositionsForDuplicatesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Verify deniedPositions for duplicates";
            }
        }
    }
}
