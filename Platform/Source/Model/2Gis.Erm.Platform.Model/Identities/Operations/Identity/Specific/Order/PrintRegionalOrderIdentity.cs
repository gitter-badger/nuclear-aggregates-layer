using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class PrintRegionalOrderIdentity : OperationIdentityBase<PrintRegionalOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.PrintRegionalOrderIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Печать регионального БЗ";
            }
        }
    }
}