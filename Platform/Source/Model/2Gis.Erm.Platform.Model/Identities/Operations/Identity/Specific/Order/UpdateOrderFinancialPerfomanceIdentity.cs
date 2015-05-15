using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class UpdateOrderFinancialPerfomanceIdentity : OperationIdentityBase<UpdateOrderFinancialPerfomanceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.UpdateOrderFinancialPerfomanceIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Обновление стоимости заказа и его позиций";
            }
        }
    }
}
