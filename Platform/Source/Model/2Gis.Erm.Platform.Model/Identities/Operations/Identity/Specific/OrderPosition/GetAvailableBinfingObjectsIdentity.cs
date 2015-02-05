using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public class GetAvailableBinfingObjectsIdentity : OperationIdentityBase<GetAvailableBinfingObjectsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetAvailableBinfingObjectsIdentity; }
        }

        public override string Description
        {
            get { return "Формирование возможных для выбора объектов привязки"; }
        }
    }
}
