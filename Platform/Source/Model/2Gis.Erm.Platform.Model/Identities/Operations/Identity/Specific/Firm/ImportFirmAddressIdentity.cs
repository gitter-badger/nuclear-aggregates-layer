using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportFirmAddressIdentity : OperationIdentityBase<ImportFirmAddressIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportFirmAddressFromServiceBusIdentity; }
        }

        public override string Description
        {
            get { return "Импорт адреса фирмы из шины"; }
        }
    }
}