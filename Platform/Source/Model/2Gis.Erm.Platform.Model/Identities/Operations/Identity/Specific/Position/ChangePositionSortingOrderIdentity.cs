using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Position
{
    public class ChangePositionSortingOrderIdentity : OperationIdentityBase<ChangePositionSortingOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ChangeSortingOrderIdentity; }
        }

        public override string Description
        {
            get { return "Смена порядка сортировки позиций (номенклатуры, прайса)"; }
        }
    }
}
