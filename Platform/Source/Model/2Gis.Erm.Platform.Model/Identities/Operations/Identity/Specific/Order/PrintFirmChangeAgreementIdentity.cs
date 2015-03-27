
namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class PrintFirmChangeAgreementIdentity : OperationIdentityBase<PrintFirmChangeAgreementIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.PrintFirmChangeAgreementIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Печать доп. соглашения (смена фирмы)";
            }
        }
    }
}
