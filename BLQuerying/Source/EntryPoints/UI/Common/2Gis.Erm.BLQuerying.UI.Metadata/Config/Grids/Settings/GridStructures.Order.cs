using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids.Settings
{
    public partial class GridStructures
    {
        public static GridMetadata Order = GridMetadata.Config
            .For<Order>()
            .AttachDataLists(
                DataLists.Configuration.DataLists.Orders,
                DataLists.Configuration.DataLists.ActiveOrders);
    }
}