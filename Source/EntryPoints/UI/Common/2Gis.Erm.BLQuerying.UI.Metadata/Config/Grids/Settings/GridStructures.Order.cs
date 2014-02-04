using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids.Settings
{
    public partial class GridStructures
    {
        public static GridStructure Order = GridStructure.Config
            .For(EntityName.Order)
            .AttachDataLists(
                DataLists.Configuration.DataLists.Orders,
                DataLists.Configuration.DataLists.ActiveOrders);
    }
}