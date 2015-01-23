namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class CreateOrderBillsIdentity : OperationIdentityBase<CreateOrderBillsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CreateOrderBillsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Создание счетов по заказу";
            }
        }
    }
}