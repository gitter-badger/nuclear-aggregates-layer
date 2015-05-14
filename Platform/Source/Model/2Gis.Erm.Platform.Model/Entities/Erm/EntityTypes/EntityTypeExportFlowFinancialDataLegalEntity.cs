using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowFinancialDataLegalEntity : EntityTypeBase<EntityTypeExportFlowFinancialDataLegalEntity>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowFinancialDataLegalEntity; }
        }

        public override string Description
        {
            get { return "ExportFlowFinancialDataLegalEntity"; }
        }
    }
}