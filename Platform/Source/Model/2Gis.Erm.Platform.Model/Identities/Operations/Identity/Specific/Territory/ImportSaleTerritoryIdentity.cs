using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Territory
{
    public class ImportSaleTerritoryIdentity : OperationIdentityBase<ImportSaleTerritoryIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportSaleTerritoryIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowGeography.SaleTerritory"; }
        }
    }
}