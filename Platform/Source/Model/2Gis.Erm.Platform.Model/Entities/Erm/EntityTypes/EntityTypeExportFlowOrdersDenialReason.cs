using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowOrdersDenialReason : EntityTypeBase<EntityTypeExportFlowOrdersDenialReason>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersDenialReason; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersDenialReason"; }
        }
    }
}