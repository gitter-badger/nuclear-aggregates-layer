using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public class ViewOrderPositionIdentity : OperationIdentityBase<ViewOrderPositionIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ViewOrderPositionIdentity; }
        }

        public override string Description
        {
            get { return "Получение информации для создания позиции заказа"; }
        }
    }
}