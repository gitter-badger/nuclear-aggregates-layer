using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Users
{
    public class UserEntityService : IUserEntityService
    {
        private readonly IFinder _finder;
        private readonly IRepository<UserEntity> _userEntityGenericRepository;
        private readonly IIdentityProvider _identityProvider;

        public UserEntityService(IRepository<UserEntity> userEntityGenericRepository, IIdentityProvider identityProvider, IFinder finder)
        {
            _userEntityGenericRepository = userEntityGenericRepository;
            _identityProvider = identityProvider;
            _finder = finder;
        }

        public int Add(UserEntity userEntity)
        {
            _identityProvider.SetFor(userEntity);
            _userEntityGenericRepository.Add(userEntity);
            return _userEntityGenericRepository.Save();
        }

        public int DeleteSharings(IEntityType entityName, long entityId)
        {
            var entityTypeId = entityName.Id;
            var sharingsToDelete = _finder.Find<UserEntity>(x => x.EntityId == entityId && x.Privilege.EntityType == entityTypeId)
                .ToArray();
            foreach (var userEntity in sharingsToDelete)
            {
                _userEntityGenericRepository.Delete(userEntity);
            }
            return _userEntityGenericRepository.Save();
        }
    }
}