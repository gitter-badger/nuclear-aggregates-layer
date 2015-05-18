using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class DeleteOrderBillsIdentity : OperationIdentityBase<DeleteOrderBillsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.DeleteOrderBillsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Удаление счетов по заказу";
            }
        }
    }
}