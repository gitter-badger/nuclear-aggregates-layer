using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services
{
    public class UserProfileDataProvider : IUserProfileDataProvider
    {
        private readonly string _webAppEndpointUrl;
        private readonly Lazy<SupportedLocalSettingsDto> _supportedLocalSettingsDto;

        public UserProfileDataProvider(string webAppEndpointUrl)
        {
            _webAppEndpointUrl = webAppEndpointUrl;
            _supportedLocalSettingsDto = new Lazy<SupportedLocalSettingsDto>(GetSupportedLocalSettings);
        }

        #region Implementation of IUserProfileDataProvider

        public LocalSettingsDto GetLocalSettings(long userCode)
        {
            LocalSettingsDto result = null;
            Exception exception = null;
            var restService = new SynchronousCallRestService(_webAppEndpointUrl);
            restService.Get<LocalSettingsDto>(
                "UserProfile/GetLocalSettings",
                new { userId = userCode },
                profileDto => result = profileDto,
                ex =>
                    {
                        exception = ex;
                        return true;
                    });
            if (exception != null)
            {
                return null;
            }

            return result;
        }

        public UserPersonalInfoDto GetPersonalInfo(long userCode)
        {
            UserPersonalInfoDto result = null;
            Exception exception = null;
            var restService = new SynchronousCallRestService(_webAppEndpointUrl);
            restService.Get<UserPersonalInfoDto>(
                    "UserProfile/GetPersonalInfo",
                    new { userId = userCode },
                    profileDto => result = profileDto,
                    ex =>
                    {
                        exception = ex;
                        return true;
                    });
            if (exception != null)
            {
                return new UserPersonalInfoDto();
            }

            return result;
        }

        public SupportedLocalSettingsDto SupportedLocalSettings
        {
            get
            {
                return _supportedLocalSettingsDto.Value;
            }
        }

        private SupportedLocalSettingsDto GetSupportedLocalSettings()
        { 
            SupportedLocalSettingsDto result = null;
            Exception exception = null;
            var restService = new SynchronousCallRestService(_webAppEndpointUrl);
            restService.Get<SupportedLocalSettingsDto>(
                    "UserProfile/GetSupportedLocalSettings",
                    null,
                    profileDto => result = profileDto,
                    ex =>
                    {
                        exception = ex;
                        return true;
                    });
            if (exception != null)
            {
                return new SupportedLocalSettingsDto { SupportedCultures = new CultureInfoDto[0], SupportedTimeZones = new TimeZoneDto[0] };
            }

            return result;
        }

        public UserProfileViewModel SaveUserProfileInfo(UserProfileViewModel viewModel)
        {
            UserProfileViewModel result = null;
            try
            {
                var restService = new SynchronousCallRestService(_webAppEndpointUrl);
                restService.Post<UserProfileViewModel>("CreateOrUpdate/UserProfile", viewModel, response => result = response);
            }
            catch (Exception ex)
            {
                // do nothing
            }
            
            return result;
        }

        #endregion
    }
}