
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class PrintCancellationAgreementIdentity : OperationIdentityBase<PrintCancellationAgreementIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.PrintCancellationAgreementIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Печать доп. соглашения (расторжение)";
            }
        }
    }
}
