using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids.Settings
{
    public static partial class GridStructures
    {
        private static readonly GridStructure[] CachedSettings;
        static GridStructures()
        {
            CachedSettings = typeof(GridStructures).Extract<GridStructure>(null).ToArray();
        }

        public static GridStructure[] Settings 
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}