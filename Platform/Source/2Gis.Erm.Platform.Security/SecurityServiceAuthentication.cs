using System;
using System.Linq;
using System.ServiceModel.Security;

using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Security.API;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Security
{
    public sealed class SecurityServiceAuthentication : IUserAuthenticationService
    {
        private readonly IFinder _finder;
        private readonly ITracer _tracer;

        public SecurityServiceAuthentication(IFinder finder, ITracer tracer)
        {
            _finder = finder;
            _tracer = tracer;
        }

        IUserIdentity IUserAuthenticationService.AuthenticateUser(string userAccount)
        {
            if (string.IsNullOrWhiteSpace(userAccount))
            {
                throw new SecurityAccessDeniedException("Ошибка при аутентификации: Получен пустой аккаунт");
            }

            try
            {
                _tracer.DebugFormat("Получаю учетную запись пользователя по аккаунту: [{0}]", userAccount);
                var userInfo = _finder.Find<User>(x => !x.IsDeleted && x.IsActive && x.Account == userAccount)
                    .Select(x => new
                    {
                        x.Id,
                        x.Account,
                        x.DisplayName
                    }).SingleOrDefault();
            
                _tracer.DebugFormat("Получил учетную запись пользователя по аккаунту: [{0}]. Полученная учетная запись: [{1}].", userAccount, (userInfo == null) ? "null" : userInfo.DisplayName);

                if (userInfo == null)
                {
                    throw new UnauthorizedAccessException("Неаутентифицированный пользователь: " + userAccount);
                }

                return new ErmUserIdentity(new UserInfo(userInfo.Id, userInfo.Account, userInfo.DisplayName));
            }
            catch (Exception ex)
            {
                throw new SecurityAccessDeniedException("Ошибка SecurityService: Аутентификация", ex);
            }
        }
    }
}
