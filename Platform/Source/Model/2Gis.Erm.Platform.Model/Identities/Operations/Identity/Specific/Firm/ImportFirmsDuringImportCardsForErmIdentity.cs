using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportFirmsDuringImportCardsForErmIdentity : OperationIdentityBase<ImportFirmsDuringImportCardsForErmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportFirmsDuringImportCardsForErmIdentity; }
        }

        public override string Description
        {
            get { return "Обновление фирм при импорте сообщения CardForERM"; }
        }
    }
}