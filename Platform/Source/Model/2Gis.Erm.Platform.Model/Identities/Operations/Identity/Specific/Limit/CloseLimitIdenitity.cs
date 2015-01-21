using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit
{
    public class CloseLimitIdenitity : OperationIdentityBase<CloseLimitIdenitity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CloseLimitIdentity; }
        }

        public override string Description
        {
            get { return "Закрытие лимита"; }
        }
    }
}