﻿using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Logging;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Security
{
    public sealed class WebCookieSignInService : ISignInService
    {
        private readonly ISecurityServiceAuthentication _securityServiceAuthentication;
        private readonly IUserIdentityLogonService _userIdentityLogonService;
        private readonly ICommonLog _logger;
        
        private readonly int _expirationInMinutes;
        private const string CookieName = "ErmTmp";

        public WebCookieSignInService(  ISecurityServiceAuthentication securityServiceAuthentication,
                                        IUserIdentityLogonService userIdentityLogonService,
                                        ICommonLog logger, 
                                        int expirationInMinutes)    
        {
            _securityServiceAuthentication = securityServiceAuthentication;
            _userIdentityLogonService = userIdentityLogonService;
            _logger = logger;

            _expirationInMinutes = expirationInMinutes;
        }

        public IUserInfo SignIn()
        {
            var context = HttpContext.Current;
            if (context.User == null || !(context.User.Identity is WindowsIdentity))
            {
                _logger.WarnEx(BLResources.WindowsIdentityNotFoundInContext);
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
                _logger.FatalFormatEx(ex, BLResources.ErorrWhileUserAuthentication, currentIdentity.Name);
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