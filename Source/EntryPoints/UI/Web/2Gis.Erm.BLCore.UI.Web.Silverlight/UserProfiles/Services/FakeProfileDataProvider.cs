using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services
{
    public class FakeProfileDataProvider : IUserProfileDataProvider
    {
        #region Implementation of IUserProfileDataProvider

        private static readonly CultureInfo DefaultCulture = new CultureInfo("ru-RU");
        private const long DefaultProfileId = 1;
        private const int DefaultTimeZoneId = 1;
        private const string DefaultTimeZoneStandartName = "N. Central Asia Standard Time";

        public LocalSettingsDto GetLocalSettings(long userCode)
        {
            return new LocalSettingsDto { CurrentCultureInfoName = DefaultCulture.Name, CurrentTimeZoneId = DefaultTimeZoneId };
        }

        public UserPersonalInfoDto GetPersonalInfo(long userCode)
        {
            return new UserPersonalInfoDto
                {
                    Address = "Address",
                    BirthDay = DateTime.Now,
                    Company = "Company",
                    DisplayName = "DisplayName",
                    Email = "Email",
                    FirstName = "FirstName",
                    Gender = UserGender.Male,
                    LastName = "LastName",
                    Manager = "Manager",
                    Mobile = "Mobile",
                    Phone = "Phone",
                    PlanetURL = "PlanetURL",
                    Position = "Position",
                };
        }

        public SupportedLocalSettingsDto SupportedLocalSettings
        {
            get
            {
                return new SupportedLocalSettingsDto
                {
                    SupportedCultures = new[] { new CultureInfoDto { Name = DefaultCulture.Name, DisplayName = DefaultCulture.DisplayName } },
                    SupportedTimeZones =
                        new[]
                        {
                            new TimeZoneDto
                                { Id = DefaultTimeZoneId, TimeZoneId = DefaultTimeZoneStandartName, DisplayName = DefaultTimeZoneStandartName }
                        }
                };
            }
        }

        public UserProfileViewModel SaveUserProfileInfo(UserProfileViewModel userProfileViewModel)
        {
            return userProfileViewModel;
        }

        #endregion
    }
}