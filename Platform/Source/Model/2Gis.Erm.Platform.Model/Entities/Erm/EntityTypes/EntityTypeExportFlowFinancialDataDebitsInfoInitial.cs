using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowFinancialDataDebitsInfoInitial : EntityTypeBase<EntityTypeExportFlowFinancialDataDebitsInfoInitial>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowFinancialDataDebitsInfoInitial; }
        }

        public override string Description
        {
            get { return "ExportFlowFinancialDataDebitsInfoInitial"; }
        }
    }
}