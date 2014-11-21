// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
// ReSharper restore CheckNamespace
{
    public sealed class SupportedLocalSettingsDto
    {
        public CultureInfoDto[] SupportedCultures { get; set; }
        public TimeZoneDto[] SupportedTimeZones { get; set; }
    }
}