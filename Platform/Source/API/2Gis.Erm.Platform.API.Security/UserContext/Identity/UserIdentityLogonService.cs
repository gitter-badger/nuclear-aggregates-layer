using System;

namespace DoubleGis.Erm.Platform.API.Security.UserContext.Identity
{
    public sealed class UserIdentityLogonService : IUserIdentityLogonService
    {
        private readonly IUserContext _userContext;

        public UserIdentityLogonService(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public void Logon(IUserIdentity identity)
        {
            var userContextAccessor = _userContext as IUserContextModifyAccessor;
            if (userContextAccessor == null)
            {
                throw new InvalidOperationException(
                    "Тип зарегистрированый в DI контейнере, как реализующий " + typeof(IUserContext).Name + ", не реализует обязательный интерфейс " +
                    typeof(IUserContextModifyAccessor).Name);
            }

            userContextAccessor.Identity = identity;
        }
    }
}
