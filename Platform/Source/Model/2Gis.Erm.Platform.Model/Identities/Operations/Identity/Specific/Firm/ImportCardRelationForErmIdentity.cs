using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportCardRelationForErmIdentity : OperationIdentityBase<ImportCardRelationForErmIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCardRelationForErmIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowCardsForErm.CardRelationForErm"; }
        }
    }
}