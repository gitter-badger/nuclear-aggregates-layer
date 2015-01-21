using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public class ImportReferenceIdentity : OperationIdentityBase<ImportReferenceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportReferenceIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowCards.Reference"; }
        }
    }
}