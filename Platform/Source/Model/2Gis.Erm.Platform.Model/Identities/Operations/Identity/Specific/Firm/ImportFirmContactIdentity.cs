using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportFirmContactIdentity : OperationIdentityBase<ImportFirmContactIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportFirmContactFromServiceBusIdentity; }
        }

        public override string Description
        {
            get { return "Импорт контакта фирмы из шины"; }
        }
    }
}