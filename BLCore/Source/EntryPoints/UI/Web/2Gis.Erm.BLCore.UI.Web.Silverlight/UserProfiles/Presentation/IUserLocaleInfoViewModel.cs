using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation
{
    public interface IUserLocaleInfoViewModel
    {
        CultureInfoDto[] SupportedCultures { get; }
        CultureInfoDto SelectedCulture { get; set; }
        TimeZoneDto[] SupportedTimeZones { get; }
        TimeZoneDto SelectedTimeZone { get; set; }
        UserLocaleInfoViewModel.SampleLocaleData SampleCultureData { get; }

        LocalSettingsDto LocalSettings { get; }
    }
}
