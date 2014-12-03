﻿using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.DI
{
    public sealed class BlFlexUiWebMetadataAssembly : IZoneAssembly<MetadataZone>,
                                                      IZoneAnchor<MetadataZone>,
                                                      IContainsType<IMetadataSource>
    {
    }
}