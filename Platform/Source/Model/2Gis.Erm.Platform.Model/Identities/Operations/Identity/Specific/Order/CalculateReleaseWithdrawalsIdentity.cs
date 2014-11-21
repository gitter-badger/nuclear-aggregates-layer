namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class CalculateReleaseWithdrawalsIdentity : OperationIdentityBase<CalculateReleaseWithdrawalsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CalculateReleaseWithdrawalsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Расчёт списаний по заказу";
            }
        }
    }
}