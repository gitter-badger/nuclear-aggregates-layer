using System;
using System.Security.Principal;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.Security
{
    public sealed class ExplicitlyIdentitySignInService : ISignInByIdentityService
    {
        private readonly ISecurityServiceAuthentication _securityServiceAuthentication;
        private readonly IUserIdentityLogonService _userIdentityLogonService;

        public ExplicitlyIdentitySignInService(ISecurityServiceAuthentication securityServiceAuthentication, IUserIdentityLogonService userIdentityLogonService)
        {
            _securityServiceAuthentication = securityServiceAuthentication;
            _userIdentityLogonService = userIdentityLogonService;
        }

        public IUserInfo SignIn(IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            var userIdentity = _securityServiceAuthentication.AuthenticateUser(IdentityUtils.GetAccount(identity));
            _userIdentityLogonService.Logon(userIdentity);

            return userIdentity;
        }
    }
}
