using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportDepCardIdentity : OperationIdentityBase<ImportDepCardIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportDepCardFromServiceBusIdentity; }
        }

        public override string Description
        {
            get { return "Импорт DepCard из шины"; }
        }
    }
}