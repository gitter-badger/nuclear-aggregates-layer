namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class SpecifyOrderDocumentsDebtIdentity : OperationIdentityBase<SpecifyOrderDocumentsDebtIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SpecifyOrderDocumentsDebtIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Редактирование информаии о задолженности документов по заказу";
            }
        }
    }
}