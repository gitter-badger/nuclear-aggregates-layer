using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class ActualizeOrderReleaseWithdrawalsIdentity : OperationIdentityBase<ActualizeOrderReleaseWithdrawalsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ActualizeOrderReleaseWithdrawalsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Актуализация списаний по заказу";
            }
        }
    }
}