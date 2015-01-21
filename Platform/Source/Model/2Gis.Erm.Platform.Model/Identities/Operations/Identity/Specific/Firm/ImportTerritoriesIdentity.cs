using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportTerritoriesIdentity : OperationIdentityBase<ImportTerritoriesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportTerritoriesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт территорий";
            }
        }
    }
}