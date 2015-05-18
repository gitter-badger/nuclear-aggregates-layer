using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeExportFlowPriceListsPriceList : EntityTypeBase<EntityTypeExportFlowPriceListsPriceList>
    {
        public override int Id
        {
            get { return EntityTypeIds.ExportFlowPriceListsPriceList; }
        }

        public override string Description
        {
            get { return "ExportFlowPriceListsPriceList"; }
        }
    }
}