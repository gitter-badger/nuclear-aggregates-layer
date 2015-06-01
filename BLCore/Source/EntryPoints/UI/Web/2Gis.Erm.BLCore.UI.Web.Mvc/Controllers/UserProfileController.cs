using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;
using TimeZone = DoubleGis.Erm.Platform.Model.Entities.Security.TimeZone;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public class UserProfileController : ControllerBase
    {
        private readonly ILocalizationSettings _localizationSettings;
        private readonly IUserProfileService _userProfileService;
        private readonly IGetUserInfoService _userInfoService;
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public UserProfileController(IMsCrmSettings msCrmSettings,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                     IIdentityServiceClientSettings identityServiceSettings,
                                     IUserContext userContext,
                                     ITracer tracer,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     ILocalizationSettings localizationSettings,
                                     IUserProfileService userProfileService,
                                     IGetUserInfoService userInfoService,
                                     IQuery query,
                                     IFinder finder)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _localizationSettings = localizationSettings;
            _userProfileService = userProfileService;
            _userInfoService = userInfoService;
            _query = query;
            _finder = finder;
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public JsonNetResult GetLocalSettings(long userId)
        {
            var profileInfo = _finder.Find(new FindSpecification<UserProfile>(x => x.UserId == userId))
                                     .Map(q => q.Select(p => new
                                         {
                                             Profile = p,
                                             p.User,
                                             p.TimeZone
                                         }))
                                     .Top();
            if (profileInfo != null)
            {
                return new JsonNetResult(new LocalSettingsDto
                {
                    CurrentCultureInfoName = new CultureInfo(profileInfo.Profile.CultureInfoLCID).Name,
                    CurrentTimeZoneId = profileInfo.TimeZone.Id
                });
            }

            return new JsonNetResult(CreateProfileDtoBasedOnAppropriateSettings(userId).Localsettings);
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public JsonNetResult GetPersonalInfo(long userId)
        {
            var profileInfo = _finder.Find(new FindSpecification<UserProfile>(x => x.UserId == userId))
                                     .Map(q => q.Select(
                                                        p => new
                                                            {
                                                                Profile = p,
                                                                p.User,
                                                                p.TimeZone
                                                            }))
                                     .Top();
            if (profileInfo != null)
            {
                return new JsonNetResult(new UserPersonalInfoDto
                {
                    Address = profileInfo.Profile.Address,
                    BirthDay = profileInfo.Profile.Birthday,
                    Company = profileInfo.Profile.Company,
                    DisplayName = profileInfo.User.DisplayName,
                    Email = profileInfo.Profile.Email,
                    FirstName = profileInfo.User.FirstName,
                    Gender = (UserGender)profileInfo.Profile.Gender,
                    LastName = profileInfo.User.LastName,
                    Manager = string.Empty,
                    Mobile = profileInfo.Profile.Mobile,
                    Phone = profileInfo.Profile.Phone,
                    PlanetURL = profileInfo.Profile.PlanetURL,
                    Position = profileInfo.Profile.Position,
                });
            }

            return new JsonNetResult(CreateProfileDtoBasedOnAppropriateSettings(userId).PersonalInfo);
        }

        [HttpGet]
        public JsonNetResult GetSupportedLocalSettings()
        {
            const int StubOffsetValue = 0;
            const string StubTimeZoneId = "";

            var supportedTimeZones = _query.For<TimeZone>().AsEnumerable().Select(tz => new
            {
                TimeZone = tz,
                TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tz.TimeZoneId)
            }).OrderByDescending(tz => tz.TimeZoneInfo.ToOffsetFromUtcInMinutes());
                
            return new JsonNetResult(new SupportedLocalSettingsDto
            {
                SupportedCultures = _localizationSettings.SupportedCultures.Select(ci => new CultureInfoDto
                {
                    DisplayName = ci.DisplayName,
                    LCID = ci.LCID,
                    Name = ci.Name,
                    ThreeLetterISOLanguageName = ci.ThreeLetterISOLanguageName,
                    TwoLetterISOLanguageName = ci.TwoLetterISOLanguageName,
                    NumberFormat = ci.NumberFormat.ToDto(),
                    DateTimeFormat = ci.DateTimeFormat.ToDto(StubOffsetValue, StubTimeZoneId),
                }).ToArray(),
                SupportedTimeZones = supportedTimeZones.Select(tz => new TimeZoneDto
                {
                    BaseUtcOffset = tz.TimeZoneInfo.BaseUtcOffset,
                    DaylightName = tz.TimeZoneInfo.DaylightName,
                    DisplayName = tz.TimeZoneInfo.DisplayName,
                    Id = tz.TimeZone.Id,
                    StandardName = tz.TimeZoneInfo.StandardName,
                    SupportsDaylightSavingTime = tz.TimeZoneInfo.SupportsDaylightSavingTime,
                    TimeZoneId = tz.TimeZone.TimeZoneId
                }).ToArray()
            });
        }

        private UserProfileViewModel CreateProfileDtoBasedOnAppropriateSettings(long userId)
        {
            var userAccountName = _finder.Find(new FindSpecification<User>(u => u.Id == userId && u.IsActive && !u.IsDeleted))
                .Map(q => q.Select(u => u.Account))
                .Top();
            if (string.IsNullOrWhiteSpace(userAccountName))
            {
                throw new NotificationException(string.Format(BLResources.UserWithIdHasNullOrEmptyAccount, userId));
            }

            var userProfileDto = new UserProfileViewModel { DomainAccountName = userAccountName, UserId = userId };

            try
            {
                var userInfo = _userInfoService.GetInfo(userAccountName);
                if (userInfo != null)
                {
                    userProfileDto.PersonalInfo = new UserPersonalInfoDto
                    {
                        Address = userInfo.Address,
                        BirthDay = userInfo.BirthDay,
                        Company = userInfo.Company,
                        DisplayName = userInfo.DisplayName,
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        Gender = (UserGender)userInfo.Gender,
                        LastName = userInfo.LastName,
                        Manager = userInfo.Manager,
                        Mobile = userInfo.Mobile,
                        Phone = userInfo.Phone,
                        PlanetURL = userInfo.PlanetURL,
                        Position = userInfo.Position
                    };
                }
            }
            catch
            {   // не удалось получить персональную информацию о пользователе через сервис персональной информации (например, AD)
                // exception просто игнорим, т.к. персональная информация - пока не является критичной - она используется только для чтения и не редактируется
                userProfileDto.PersonalInfo = new UserPersonalInfoDto();
            }


            var appropriateUserLocaleInfo = _userProfileService.GetUserProfile(userId).UserLocaleInfo;
            var appropriateTimeZone = _finder.Find(new FindSpecification<TimeZone>(tz => tz.TimeZoneId.Equals(appropriateUserLocaleInfo.UserTimeZoneInfo.Id))).Top();
            if (appropriateTimeZone == null)
            {
                throw new NotificationException(string.Format(BLResources.TimeZoneIsUnsupported, appropriateUserLocaleInfo.UserTimeZoneInfo.StandardName));
            }

            userProfileDto.Localsettings = new LocalSettingsDto
            {
                CurrentCultureInfoName = appropriateUserLocaleInfo.UserCultureInfo.Name,
                CurrentTimeZoneId = appropriateTimeZone.Id
            };

            return userProfileDto;
        }
    }
}