using System;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.Security
{
    public sealed class UserImpersonationService : IUserImpersonationService
    {
        private readonly ISecurityServiceAuthentication _securityServiceAuthentication;
        private readonly IUserIdentityLogonService _userIdentityLogonService;

        public UserImpersonationService(ISecurityServiceAuthentication securityServiceAuthentication, IUserIdentityLogonService userIdentityLogonService)
        {
            _securityServiceAuthentication = securityServiceAuthentication;
            _userIdentityLogonService = userIdentityLogonService;
        }

        public IUserInfo ImpersonateAsUser(string userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException("userAccount");
            }

            var userIdentity = _securityServiceAuthentication.AuthenticateUser(userAccount);
            _userIdentityLogonService.Logon(userIdentity);

            return userIdentity;
        }

    }
}
