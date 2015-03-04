using System;
using System.Collections.Generic;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.Settings;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.WCF.MoDi
{
    public sealed class MoneyDistributionSettingsAspect : ISettingsAspect, IMoneyDistributionSettings
    {
        private readonly DateTime _firstApril = new DateTime(2013, 04, 01);

        private readonly IReadOnlyDictionary<PlatformEnum, PlatformsExtended> _extendedPlatformMap = new Dictionary<PlatformEnum, PlatformsExtended>
        {
            { PlatformEnum.Independent, PlatformsExtended.None },
            { PlatformEnum.Desktop, PlatformsExtended.Desktop },
            { PlatformEnum.Mobile, PlatformsExtended.Mobile },
            { PlatformEnum.Api, PlatformsExtended.Api },
            { PlatformEnum.Online, PlatformsExtended.Online },
        };

        private readonly IReadOnlyDictionary<PlatformsExtended, PlatformEnum> _platformMap = new Dictionary<PlatformsExtended, PlatformEnum>
        {
            { PlatformsExtended.None, PlatformEnum.Independent },
            { PlatformsExtended.Desktop, PlatformEnum.Desktop },
            { PlatformsExtended.Mobile, PlatformEnum.Mobile },
            { PlatformsExtended.Api, PlatformEnum.Api },
            { PlatformsExtended.ApiOnline, PlatformEnum.Api },
            { PlatformsExtended.ApiPartner, PlatformEnum.Api },
            { PlatformsExtended.Online, PlatformEnum.Online },
        };

        private readonly CultureInfo _defaultCultureInfo = CultureInfo.GetCultureInfo("ru-RU");

        public DateTime FirstApril
        {
            get { return _firstApril; }
        }

        public long UkBouId
        {
            get { return 129; }
        }

        public long UkOrganizationUnitId
        {
            get { return 128; }
        }

        public IReadOnlyDictionary<PlatformEnum, PlatformsExtended> ExtendedPlatformMap
        {
            get { return _extendedPlatformMap; }
        }

        public IReadOnlyDictionary<PlatformsExtended, PlatformEnum> PlatformMap
        {
            get { return _platformMap; }
        }

        public CultureInfo DefaultCultureInfo
        {
            get { return _defaultCultureInfo; }
        }
    }
}