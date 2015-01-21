using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit
{
    public class CalculateLimitIncreasingIdentity : OperationIdentityBase<CalculateLimitIncreasingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CalculateLimitIncreasingIdentity; }
        }

        public override string Description
        {
            get { return "Рассчет суммы увеличения лимита"; }
        }
    }
}