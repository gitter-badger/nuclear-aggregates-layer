using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Users
{
    public sealed class DeactivateUserAggregateService : IDeactivateUserAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IRepository<UserProfile> _userProfileGenericRepository;
        private readonly IRepository<UserRole> _userRoleGenericRepository;
        private readonly IRepository<User> _userGenericRepository;

        public DeactivateUserAggregateService(IOperationScopeFactory operationScopeFactory,
                                              IRepository<UserProfile> userProfileGenericRepository,
                                              IRepository<UserRole> userRoleGenericRepository,
                                              IRepository<User> userGenericRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _userProfileGenericRepository = userProfileGenericRepository;
            _userRoleGenericRepository = userRoleGenericRepository;
            _userGenericRepository = userGenericRepository;
        }

        public void Deactivate(User user, UserProfile profile, IEnumerable<UserRole> userRoleRelations)
        {
            Deactivate(user);
            Delete(userRoleRelations);

            if (profile != null)
            {
                Deactivate(profile);
            }
        }

        private void Deactivate(User user)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, User>())
            {
                user.IsActive = false;
                _userGenericRepository.Update(user);
                operationScope.Updated<User>(user.Id);

                _userGenericRepository.Save();
                operationScope.Complete();
            }
        }

        private void Delete(IEnumerable<UserRole> roles)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, UserRole>())
            {
                foreach (var userRole in roles)
                {
                    _userRoleGenericRepository.Delete(userRole);
                    operationScope.Deleted<UserRole>(userRole.Id);
                }

                _userRoleGenericRepository.Save();
                operationScope.Complete();
            }
        }

        private void Deactivate(UserProfile userProfile)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, UserProfile>())
            {
                userProfile.IsActive = false;
                _userProfileGenericRepository.Update(userProfile);
                operationScope.Updated<UserProfile>(userProfile.Id);

                _userProfileGenericRepository.Save();
                operationScope.Complete();
            }
        }
    }
}