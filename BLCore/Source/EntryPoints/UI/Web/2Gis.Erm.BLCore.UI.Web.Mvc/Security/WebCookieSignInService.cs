using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using Newtonsoft.Json;

using NuClear.Security.API;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Security
{
    public sealed class WebCookieSignInService : ISignInService
    {
        private readonly IUserAuthenticationService _securityServiceAuthentication;
        private readonly IUserIdentityLogonService _userIdentityLogonService;
        private readonly ITracer _tracer;
        
        private readonly int _expirationInMinutes;
        private const string CookieName = "ErmTmp";

        public WebCookieSignInService(  IUserAuthenticationService securityServiceAuthentication,
                                        IUserIdentityLogonService userIdentityLogonService,
                                        ITracer tracer, 
                                        int expirationInMinutes)    
        {
            _securityServiceAuthentication = securityServiceAuthentication;
            _userIdentityLogonService = userIdentityLogonService;
            _tracer = tracer;

            _expirationInMinutes = expirationInMinutes;
        }

        public IUserInfo SignIn()
        {
            var context = HttpContext.Current;
            if (context.User == null || !(context.User.Identity is WindowsIdentity))
            {
                _tracer.Warn(BLResources.WindowsIdentityNotFoundInContext);
                throw new UnauthorizedAccessException(BLResources.WindowsIdentityNotFoundInContext);
            }

            var currentIdentity = (WindowsIdentity)context.User.Identity;

            try
            {
                IUserIdentity userIdentity = null;
                var authCookie = context.Request.Cookies[CookieName];

                if (authCookie != null)
                {
                    try
                    {
                        var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        var identityInfo = JsonConvert.DeserializeObject<UserInfo>(authTicket.UserData);
                        userIdentity = new ErmUserIdentity(identityInfo);
                    }
                    catch (CryptographicException)
                    {
                        // обнуляем cookie если не смогли его расшифровать
                        authCookie = null;
                    }
                }

                var userAccountName = IdentityUtils.GetAccount(currentIdentity);
                if (authCookie == null || (!userIdentity.Account.Equals(userAccountName, StringComparison.OrdinalIgnoreCase)))
                {
                    userIdentity = _securityServiceAuthentication.AuthenticateUser(userAccountName);
                    CreateCookie(context.Response, currentIdentity.Name, CookieName, _expirationInMinutes, userIdentity.UserData);
                }

                _userIdentityLogonService.Logon(userIdentity);

                return userIdentity;
            }
            catch (Exception ex)
            {
                _tracer.FatalFormat(ex, BLResources.ErorrWhileUserAuthentication, currentIdentity.Name);
                throw new UnauthorizedAccessException(string.Format(BLResources.ErorrWhileUserAuthentication, currentIdentity.Name), ex);
            }
        }

        private static void CreateCookie(HttpResponse httpResponse, string userName, string cookieName, int expirationInMinutes, UserInfo identityInfo)
        {
            var identitySerialized = JsonConvert.SerializeObject(identityInfo);
            var currentTime = DateTime.UtcNow;
            var ticket = new FormsAuthenticationTicket(
                                    1,
                                    userName,
                                    currentTime,
                                    currentTime.AddMinutes(expirationInMinutes),
                                    false, 
                                    identitySerialized, 
                                    "/");

            var cookieString = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(cookieName, cookieString)
            {
                Expires = ticket.Expiration,
                Path = ticket.CookiePath
            };

            httpResponse.Cookies.Remove(cookieName);
            httpResponse.Cookies.Add(cookie);
        }
    }
}