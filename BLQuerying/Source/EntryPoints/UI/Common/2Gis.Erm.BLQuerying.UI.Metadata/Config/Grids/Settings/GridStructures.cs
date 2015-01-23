using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLQuerying.UI.Metadata.Config.Grids.Settings
{
    public static partial class GridStructures
    {
        private static readonly GridMetadata[] CachedSettings;
        static GridStructures()
        {
            CachedSettings = typeof(GridStructures).Extract<GridMetadata>(null).ToArray();
        }

        public static GridMetadata[] Settings 
        {
            get
            {
                return CachedSettings;
            }
        }
    }
}