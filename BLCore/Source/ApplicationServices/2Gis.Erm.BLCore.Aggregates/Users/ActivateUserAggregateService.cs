using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Users
{
    public sealed class ActivateUserAggregateService : IActivateUserAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        
        private readonly IRepository<UserProfile> _userProfileGenericRepository;
        private readonly IRepository<User> _userGenericRepository;

        public ActivateUserAggregateService(IOperationScopeFactory operationScopeFactory,
                                            IRepository<UserProfile> userProfileGenericRepository,
                                            IRepository<User> userGenericRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _userProfileGenericRepository = userProfileGenericRepository;
            _userGenericRepository = userGenericRepository;
        }

        public void Activate(User user, UserProfile profile)
        {
            Activate(user);
            if (profile != null)
            {
                Activate(profile);
            }
        }

        private void Activate(User user)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, User>())
            {
                user.IsActive = true;
                _userGenericRepository.Update(user);
                scope.Updated<User>(user.Id);
                _userGenericRepository.Save();

                scope.Complete();
            }
        }

        private void Activate(UserProfile userProfile)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, UserProfile>())
            {
                userProfile.IsActive = true;
                _userProfileGenericRepository.Update(userProfile);
                scope.Updated<UserProfile>(userProfile.Id);
                _userProfileGenericRepository.Save();

                scope.Complete();
            }
        }
    }
}