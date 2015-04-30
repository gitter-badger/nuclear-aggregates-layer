using DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.Users.Operations
{
    public sealed class UserAppendRoleAggregateService : IUserAppendRoleAggregateService
    {
        private readonly IRepository<UserRole> _userRoleRelationsRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public UserAppendRoleAggregateService(
            IRepository<UserRole> userRoleRelationsRepository, 
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _userRoleRelationsRepository = userRoleRelationsRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void AppendRole(User user, long roleId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, User, Role>())
            {
                var userRole = new UserRole { UserId = user.Id, RoleId = roleId };
                _identityProvider.SetFor(userRole);
                _userRoleRelationsRepository.Add(userRole);
                _userRoleRelationsRepository.Save();

                scope.Added<UserRole>(userRole.Id)
                     .Updated<User>(user.Id)
                     .Updated<Role>(roleId)
                     .Complete();
            }
        }
    }
}
