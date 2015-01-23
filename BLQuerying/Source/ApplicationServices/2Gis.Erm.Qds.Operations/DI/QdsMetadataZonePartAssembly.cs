﻿using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Qds.Operations.DI
{
    public sealed class QdsMetadataZonePartAssembly : IZoneAssembly<MetadataZone>,
                                                      IZoneAnchor<MetadataZone>,
                                                      IContainsType<IMetadataSource>
    {
    }
}