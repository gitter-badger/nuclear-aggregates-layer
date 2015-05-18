using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Building
{
    public class ImportBuildingIdentity : OperationIdentityBase<ImportBuildingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportBuildingIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowGeography.Building"; }
        }
    }
}