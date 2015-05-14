using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowOrdersTheme : EntityTypeBase<EntityTypeExportFlowOrdersTheme>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersTheme; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersTheme"; }
        }
    }
}