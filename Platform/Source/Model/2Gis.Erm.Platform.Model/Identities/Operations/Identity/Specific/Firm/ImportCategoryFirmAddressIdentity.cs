using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportCategoryFirmAddressIdentity : OperationIdentityBase<ImportCategoryFirmAddressIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCategoryFirmAddressFromServiceBusIdentity; }
        }

        public override string Description
        {
            get { return "Импорт связи адреса фирмы и рубрики из шины"; }
        }
    }
}