using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowOrdersThemeBranch : EntityTypeBase<EntityTypeExportFlowOrdersThemeBranch>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersThemeBranch; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersThemeBranch"; }
        }
    }
}