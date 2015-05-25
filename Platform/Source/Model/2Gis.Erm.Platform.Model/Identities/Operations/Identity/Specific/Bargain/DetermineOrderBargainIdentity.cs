using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain
{
    public sealed class DetermineOrderBargainIdentity : OperationIdentityBase<DetermineOrderBargainIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.DetermineOrderBargainIdentity; }
        }

        public override string Description
        {
            get { return "Определение подходящего договора для заказа"; }
        }
    }
}