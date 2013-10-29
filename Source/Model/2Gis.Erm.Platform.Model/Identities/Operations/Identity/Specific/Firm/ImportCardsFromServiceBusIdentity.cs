namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportCardsFromServiceBusIdentity : OperationIdentityBase<ImportCardsFromServiceBusIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportCardsFromServiceBusIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт фирм из шины интеграции";
            }
        }
    }
}