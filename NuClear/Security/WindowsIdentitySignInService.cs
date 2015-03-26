using System;
using System.Security.Principal;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.Security
{
    /// <summary>
    /// Аутентифицирует пользователя в системе на основе windowsidentity пользователя под чей учеткой выполняется код (вызывается метод SignIn)
    /// После аутентификации описатель пользователя сохраняется в UserContext
    /// </summary>
    public class WindowsIdentitySignInService : ISignInService
    {
        private readonly ExplicitlyIdentitySignInService _identitySignInService;

        public WindowsIdentitySignInService(ISecurityServiceAuthentication securityServiceAuthentication, IUserIdentityLogonService userIdentityLogonService)
        {
            _identitySignInService = new ExplicitlyIdentitySignInService(securityServiceAuthentication, userIdentityLogonService);
        }

        public IUserInfo SignIn()
        {
            var currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity == null)
            {
                throw new UnauthorizedAccessException("Необходимо запустить NT Service под аккаунтом зарегистрированного в системе пользователя");
            }
            
            return _identitySignInService.SignIn(currentIdentity);
        }
    }
}