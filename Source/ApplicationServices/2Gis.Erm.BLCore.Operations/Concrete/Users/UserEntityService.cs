using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Security;

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

        public int DeleteSharings(EntityName entityName, long entityId)
        {
            var sharingsToDelete = _finder.Find<UserEntity>(x => x.EntityId == entityId && x.Privilege.EntityType == (int)entityName)
                .ToArray();
            foreach (var userEntity in sharingsToDelete)
            {
                _userEntityGenericRepository.Delete(userEntity);
            }
            return _userEntityGenericRepository.Save();
        }
    }
}