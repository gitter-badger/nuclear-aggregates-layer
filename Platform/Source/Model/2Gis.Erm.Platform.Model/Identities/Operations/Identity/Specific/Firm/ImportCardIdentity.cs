using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportCardIdentity : OperationIdentityBase<ImportCardIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCardsFromServiceBusIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowCards.Card"; }
        }
    }
}