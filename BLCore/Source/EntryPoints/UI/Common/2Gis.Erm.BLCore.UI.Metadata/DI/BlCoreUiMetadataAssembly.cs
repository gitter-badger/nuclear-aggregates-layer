﻿using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.UI.Metadata.DI
{
    public class BlCoreUIMetadataAssembly : IZoneAssembly<MetadataZone>,
                                            IZoneAnchor<MetadataZone>,
                                            IContainsType<IMetadataSource>
    {
    }
}