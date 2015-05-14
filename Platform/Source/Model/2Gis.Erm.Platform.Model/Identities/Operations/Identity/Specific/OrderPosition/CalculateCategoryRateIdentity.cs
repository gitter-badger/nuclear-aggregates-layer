using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public class CalculateCategoryRateIdentity : OperationIdentityBase<CalculateCategoryRateIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CalculateCategoryRateIdentity; }
        }

        public override string Description
        {
            get { return "Расчет коэффициента группы рубрик для позиции заказа"; }
        }
    }
}