using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;

using NuClear.Security.API;
using NuClear.Security.API.UserContext.Profile;
using DoubleGis.Erm.Platform.Common.Caching;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Users
{
    public sealed class UserProfileService : IUserProfileService
    {
        private const string ProfilesCacheKeyTemplate = "security:userprofile-foruser-{0}";
        private static readonly TimeSpan CachedProfileExpiration = TimeSpan.FromSeconds(60);

        private readonly IUserRepository _userRepository;
        private readonly ICacheAdapter _cacheAdapter;

        public UserProfileService(ICacheAdapter cacheAdapter, IUserRepository userRepository)
        {
            _cacheAdapter = cacheAdapter;
            _userRepository = userRepository;
        }

        public IUserProfile GetUserProfile(long userCode)
        {
            var profileCacheKey = string.Format(ProfilesCacheKeyTemplate, userCode);

            var userProfile = _cacheAdapter.Get<ErmUserProfile>(profileCacheKey);
            if (userProfile != null)
            {
                return userProfile;
            }

            var localeInfo = _userRepository.GetUserLocaleInfo(userCode) ?? LocaleInfo.Default;
            userProfile = new ErmUserProfile(userCode, localeInfo);

            _cacheAdapter.Add(profileCacheKey, userProfile, CachedProfileExpiration);

            return userProfile;
        }
    }
}