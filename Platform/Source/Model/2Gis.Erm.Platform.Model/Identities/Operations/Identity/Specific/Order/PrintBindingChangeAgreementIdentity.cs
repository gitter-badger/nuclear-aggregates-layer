
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class PrintBindingChangeAgreementIdentity : OperationIdentityBase<PrintBindingChangeAgreementIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.PrintBindingChangeAgreementIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Печать доп. соглашения (смена привязки)";
            }
        }
    }
}
