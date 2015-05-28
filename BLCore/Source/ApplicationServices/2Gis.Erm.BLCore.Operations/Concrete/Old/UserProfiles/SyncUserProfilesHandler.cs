using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.UserProfiles;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.UserProfiles
{
    public sealed class SyncUserProfilesHandler : RequestHandler<SyncUserProfilesRequest, EmptyResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGetUserInfoService _getUserInfoService;

        public SyncUserProfilesHandler(IUserRepository userRepository, IGetUserInfoService getUserInfoService)
        {
            _userRepository = userRepository;
            _getUserInfoService = getUserInfoService;
        }

        protected override EmptyResponse Handle(SyncUserProfilesRequest request)
        {
            var profileDtos = _userRepository.GetAllUserProfiles();

            foreach (var profileDto in profileDtos)
            {
                ADUserProfile adUserProfile;
                if (!_getUserInfoService.TryGetInfo(profileDto.UserAccountName, out adUserProfile))
                {
                    continue;
                }

                var userProfile = profileDto.UserProfile;

                userProfile.Address = adUserProfile.Address;
                userProfile.Birthday = adUserProfile.BirthDay;
                userProfile.Company = adUserProfile.Company;
                userProfile.Email = adUserProfile.Email;
                userProfile.Gender = (int)adUserProfile.Gender;
                userProfile.Mobile = adUserProfile.Mobile;
                userProfile.Phone = adUserProfile.Phone;
                userProfile.PlanetURL = adUserProfile.PlanetURL;
                userProfile.Position = adUserProfile.Position;
            }

            _userRepository.UpdateUserProfiles(profileDtos);

            return Response.Empty;
        }
    }
}
