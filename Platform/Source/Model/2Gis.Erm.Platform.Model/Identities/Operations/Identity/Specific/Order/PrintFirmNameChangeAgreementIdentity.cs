
namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class PrintFirmNameChangeAgreementIdentity : OperationIdentityBase<PrintFirmNameChangeAgreementIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.PrintFirmNameChangeAgreementIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Печать доп. соглашения (смена названия фирмы)";
            }
        }
    }
}
