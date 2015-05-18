using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class SetOrderDocumentsDebtIdentity : OperationIdentityBase<SetOrderDocumentsDebtIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SetOrderDocumentsDebtIdentity;
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