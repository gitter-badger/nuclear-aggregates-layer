using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class SetInspectorIdentity : OperationIdentityBase<SetInspectorIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SetInspectorIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Указать проверяющего";
            }
        }
    }
}