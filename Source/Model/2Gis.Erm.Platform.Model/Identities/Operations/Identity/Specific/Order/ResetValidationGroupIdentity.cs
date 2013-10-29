namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class ResetValidationGroupIdentity : OperationIdentityBase<ResetValidationGroupIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ResetValidationGroupIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Сброс группы проверок для заказа";
            }
        }
    }
}
