using System;
using System.Globalization;
using System.Web;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Security
{
    /// <summary>
    /// Создает значения по умолчанию для UserContext (т.е. identity и userprofile)
    /// Значения по умолчанию необходимы, до момента выставления реальных данных о пользователе в UserContext - до аутентификации
    /// Значения по умолчанию для identity все hardcoded
    /// Значения по умолчанию для userprofile: культуру пытаемся вытащить из браузера, пользователя, timezone используется default
    /// </summary>
    public class WebDefaultUserContextConfigurator : IDefaultUserContextConfigurator
    {
        private readonly IUserContext _userContext;
        private readonly ICommonLog _logger;
        private readonly String _uid;

        public WebDefaultUserContextConfigurator(IUserContext userContext, ICommonLog logger)
        {
            _userContext = userContext;
            _logger = logger;
            _uid = Guid.NewGuid().ToString();
        }

        private const int UnauthenticatedUserCode = -1;
        private const String UnauthenticatedUserAccount = "UnauthenticatedUserAccount";
        private const String UnauthenticatedUserDisplayName = "UnauthenticatedUserDisplayName";

        public void Configure(HttpRequest processingHttpRequest)
        {
            var userContextAccessor = _userContext as IUserContextModifyAccessor;
            if (userContextAccessor == null)
            {
                var msg = "Тип зарегистрированый в DI контейнере, как реализующий " + typeof(IUserContext).Name + ", не реализует обязательный интерфейс " + typeof(IUserContextModifyAccessor).Name;
                _logger.Fatal(msg);
                throw new InvalidOperationException(msg);
            }

            if (_userContext.Profile == null)
            {
                var defaultLocale = LocaleInfo.Default;
            
                var culture = GetCultureForRequest(processingHttpRequest);
                var userProfile = culture == null ? 
                                    new ErmUserProfile(UnauthenticatedUserCode, defaultLocale) : 
                                    new ErmUserProfile(UnauthenticatedUserCode, new LocaleInfo(defaultLocale.UserTimeZoneInfo.Id, culture.LCID)); 
                userContextAccessor.Profile = userProfile;
            }

            if (_userContext.Identity == null)
            {
                var userIdentity = new ErmUserIdentity(new UserInfo(UnauthenticatedUserCode, UnauthenticatedUserAccount + _uid, UnauthenticatedUserDisplayName + _uid));
                userContextAccessor.Identity = userIdentity;
            }           
        }

        /// <summary>
        /// Вытаскивание локали из http запроса
        /// Поддерживаемые культуры может передавать браузер
        /// </summary>
        private static CultureInfo GetCultureForRequest(HttpRequest processingHttpRequest)
        {
            if (processingHttpRequest.UserLanguages == null || processingHttpRequest.UserLanguages.Length == 0)
            {
                return null;
            }

            foreach (var language in processingHttpRequest.UserLanguages)
            {
                if (language != null)
                {
                    try
                    {
                        return new CultureInfo(language);
                    }
                    catch
                    {
                        // do nothing
                    }
                }
            }

            return null;
        }
    }
}