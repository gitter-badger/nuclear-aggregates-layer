namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportFlowCardsForErmIdentity : OperationIdentityBase<ImportFlowCardsForErmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportFlowCardsForErmIdentity; }
        }

        public override string Description
        {
            get { return "Импорт карточек из потока flowCardsForErmIdentity"; }
        }
    }
}