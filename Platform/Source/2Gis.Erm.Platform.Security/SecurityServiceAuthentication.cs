using System;
using System.Linq;
using System.ServiceModel.Security;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Security
{
    public sealed class SecurityServiceAuthentication : ISecurityServiceAuthentication
    {
        private readonly IFinder _finder;
        private readonly ICommonLog _logger;

        public SecurityServiceAuthentication(IFinder finder, ICommonLog logger)
        {
            _finder = finder;
            _logger = logger;
        }

        IUserIdentity ISecurityServiceAuthentication.AuthenticateUser(string userAccount)
        {
            if (string.IsNullOrWhiteSpace(userAccount))
            {
                throw new SecurityAccessDeniedException("Ошибка при аутентификации: Получен пустой аккаунт");
            }

            try
            {
                _logger.DebugFormat("Получаю учетную запись пользователя по аккаунту: [{0}]", userAccount);
                var userInfo = _finder.Find<User>(x => !x.IsDeleted && x.IsActive && x.Account == userAccount)
                    .Select(x => new
                    {
                        x.Id,
                        x.Account,
                        x.DisplayName
                    }).SingleOrDefault();
            
                _logger.DebugFormat("Получил учетную запись пользователя по аккаунту: [{0}]. Полученная учетная запись: [{1}].", userAccount, (userInfo == null) ? "null" : userInfo.DisplayName);

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
