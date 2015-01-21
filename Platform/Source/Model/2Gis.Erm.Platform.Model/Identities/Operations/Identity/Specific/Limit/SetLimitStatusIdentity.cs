using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit
{
    public class SetLimitStatusIdentity : OperationIdentityBase<SetLimitStatusIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SetLimitStatusIdentity; }
        }

        public override string Description
        {
            get { return "Смена сатуса лимита"; }
        }
    }
}