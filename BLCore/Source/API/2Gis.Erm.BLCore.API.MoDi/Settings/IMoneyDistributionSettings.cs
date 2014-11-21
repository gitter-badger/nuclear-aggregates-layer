using System;
using System.Collections.Generic;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.Enums;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.MoDi.Settings
{
    public interface IMoneyDistributionSettings : ISettings
    {
        DateTime FirstApril { get; }
        long UkBouId { get; }
        long UkOrganizationUnitId { get; }
        IReadOnlyDictionary<PlatformEnum, PlatformsExtended> ExtendedPlatformMap { get; }
        IReadOnlyDictionary<PlatformsExtended, PlatformEnum> PlatformMap { get; }
        CultureInfo DefaultCultureInfo { get; }
    }
}