using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public class CalculateOrderPositionPricePerUnitIdentity : OperationIdentityBase<CalculateOrderPositionPricePerUnitIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CalculateOrderPositionPricePerUnitIdentity; }
        }

        public override string Description
        {
            get { return "Расчет цены за единицу позиции заказа"; }
        }
    }
}
