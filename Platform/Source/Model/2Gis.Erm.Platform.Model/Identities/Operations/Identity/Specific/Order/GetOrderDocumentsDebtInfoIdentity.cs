namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class GetOrderDocumentsDebtInfoIdentity : OperationIdentityBase<GetOrderDocumentsDebtInfoIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.GetOrderDocumentsDebtInfoIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Получение информаии о задолженности документов по заказу";
            }
        }
    }
}