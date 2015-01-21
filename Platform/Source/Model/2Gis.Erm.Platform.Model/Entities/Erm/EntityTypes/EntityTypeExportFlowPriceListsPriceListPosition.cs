using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowPriceListsPriceListPosition : EntityTypeBase<EntityTypeExportFlowPriceListsPriceListPosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowPriceListsPriceListPosition; }
        }

        public override string Description
        {
            get { return "ExportFlowPriceListsPriceListPosition"; }
        }
    }
}