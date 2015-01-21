using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public sealed class ChangeOrderPositionBindingObjectsIdentity : OperationIdentityBase<ChangeOrderPositionBindingObjectsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ChangeOrderPositionBindingObjectsIdentity; }
        }

        public override string Description
        {
            get { return "Смена объектов привязки у связей позиции заказа с рекламой"; }
        }
    }
}