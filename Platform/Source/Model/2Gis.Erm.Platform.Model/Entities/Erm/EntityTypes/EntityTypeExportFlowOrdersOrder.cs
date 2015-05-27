using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowOrdersOrder : EntityTypeBase<EntityTypeExportFlowOrdersOrder>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersOrder; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersOrder"; }
        }
    }
}