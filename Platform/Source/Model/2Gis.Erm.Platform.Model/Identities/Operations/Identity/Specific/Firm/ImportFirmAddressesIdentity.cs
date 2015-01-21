using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportFirmAddressesIdentity : OperationIdentityBase<ImportFirmAddressesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportFirmAddresses;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт адресов фирм";
            }
        }
    }
}