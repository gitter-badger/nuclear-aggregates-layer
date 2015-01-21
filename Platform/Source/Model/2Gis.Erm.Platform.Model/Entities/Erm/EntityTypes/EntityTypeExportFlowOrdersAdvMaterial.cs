using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowOrdersAdvMaterial : EntityTypeBase<EntityTypeExportFlowOrdersAdvMaterial>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersAdvMaterial; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersAdvMaterial"; }
        }
    }
}