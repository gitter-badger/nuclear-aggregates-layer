using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Category
{
    public class ImportRubricIdentity : OperationIdentityBase<ImportRubricIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportRubricIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowRubrics.Rubric"; }
        }
    }
}