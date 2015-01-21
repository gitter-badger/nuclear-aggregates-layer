using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EtntityTypeExportFlowOrdersResource : EntityTypeBase<EtntityTypeExportFlowOrdersResource>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersResource; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersResource"; }
        }
    }
}