using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class GetOrderDocumentsDebtIdentity : OperationIdentityBase<GetOrderDocumentsDebtIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.GetOrderDocumentsDebtIdentity;
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