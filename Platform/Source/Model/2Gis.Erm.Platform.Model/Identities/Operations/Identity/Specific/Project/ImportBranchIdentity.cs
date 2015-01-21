using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Project
{
    public class ImportBranchIdentity : OperationIdentityBase<ImportBranchIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportBranchIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowGeoClassifier.Branch"; }
        }
    }
}