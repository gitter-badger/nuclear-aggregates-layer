namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm
{
    public sealed class ImportTerritoriesFromDgppIdentity : OperationIdentityBase<ImportTerritoriesFromDgppIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ImportTerritoriesFromDgppIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Импорт территорий из ДГПП";
            }
        }
    }
}