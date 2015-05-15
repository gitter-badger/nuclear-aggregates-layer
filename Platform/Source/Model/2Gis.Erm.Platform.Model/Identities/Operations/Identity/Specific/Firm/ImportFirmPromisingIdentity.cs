using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportFirmPromisingIdentity : OperationIdentityBase<ImportFirmPromisingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportFirmPromising;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт перспективности фирм";
            }
        }
    }
}