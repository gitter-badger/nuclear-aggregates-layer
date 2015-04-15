using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition
{
    [DataContract]
    public sealed class VerifyDeniedPositionsForSymmetryIdentity : OperationIdentityBase<VerifyDeniedPositionsForSymmetryIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.VerifyDeniedPositionsForSymmetryIdentity;
            }
        }

        public override string Description
        {
            get { return "Verify deniedPositions for symmetry"; }
        }
    }
}
