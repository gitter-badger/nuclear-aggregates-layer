using System;
using System.Globalization;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Profile
{
    public sealed class LocaleInfo
    {
        public static readonly LocaleInfo Default = new LocaleInfo("N. Central Asia Standard Time", 1049);

        public TimeZoneInfo UserTimeZoneInfo { get; private set; }
        public CultureInfo UserCultureInfo { get; private set; }

        public LocaleInfo(string timeZoneInfoId, int cultureInfoLcid)
        {
            UserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfoId);
            UserCultureInfo = new CultureInfo(cultureInfoLcid);
        }
    }
}