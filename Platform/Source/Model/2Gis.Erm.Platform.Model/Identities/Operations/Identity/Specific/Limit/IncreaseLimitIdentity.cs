using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit
{
    public class IncreaseLimitIdentity : OperationIdentityBase<IncreaseLimitIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.IncreaseLimitIdentity; }
        }

        public override string Description
        {
            get { return "Увеличение суммы лимита"; }
        }
    }
}