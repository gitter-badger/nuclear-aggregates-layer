using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit
{
    public class RecalculateLimitIdentity : OperationIdentityBase<RecalculateLimitIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.RecalculateLimitIdentity; }
        }

        public override string Description
        {
            get { return "Пересчет лимита"; }
        }
    }
}