using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportReferenceItemIdentity : OperationIdentityBase<ImportReferenceItemIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportReferenceItemIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowCards.ReferenceItem"; }
        }
    }
}