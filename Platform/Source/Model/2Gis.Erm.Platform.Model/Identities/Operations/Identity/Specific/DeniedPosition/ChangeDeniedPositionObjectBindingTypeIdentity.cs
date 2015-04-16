using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition
{
    [DataContract]
    public sealed class ChangeDeniedPositionObjectBindingTypeIdentity : OperationIdentityBase<ChangeDeniedPositionObjectBindingTypeIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeDeniedPositionObjectBindingTypeIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Change denied position object binding type";
            }
        }
    }
}
