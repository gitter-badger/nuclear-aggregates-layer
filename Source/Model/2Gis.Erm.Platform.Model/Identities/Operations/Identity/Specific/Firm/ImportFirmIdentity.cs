namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportFirmIdentity : OperationIdentityBase<ImportFirmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportFirmIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт фирмы из внешней системы";
            }
        }
    }
}