using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowCardExtensionsCardCommercial : EntityTypeBase<EntityTypeExportFlowCardExtensionsCardCommercial>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowCardExtensionsCardCommercial; }
        }

        public override string Description
        {
            get { return "ExportFlowCardExtensionsCardCommercial"; }
        }
    }
}