using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportCardRelationIdentity : OperationIdentityBase<ImportCardRelationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportCardRelationIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowCards.CardRelation"; }
        }
    }
}