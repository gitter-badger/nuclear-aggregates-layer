using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowFinancialDataClient : EntityTypeBase<EntityTypeExportFlowFinancialDataClient>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowFinancialDataClient; }
        }

        public override string Description
        {
            get { return "ExportFlowFinancialDataClient"; }
        }
    }
}