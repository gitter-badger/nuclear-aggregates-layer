using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowOrdersInvoice : EntityTypeBase<EntityTypeExportFlowOrdersInvoice>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowOrdersInvoice; }
        }

        public override string Description
        {
            get { return "ExportFlowOrdersInvoice"; }
        }
    }
}