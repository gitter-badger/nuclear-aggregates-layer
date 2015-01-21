using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class RepairOutdatedIdentity : OperationIdentityBase<RepairOutdatedIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.RepairOutdatedIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Актуализировать используемый прайс-лист";
            }
        }
    }
}