using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditUserProfileHandler : RequestHandler<EditRequest<UserProfile>, EmptyResponse>
    {
        private readonly IUserRepository _userRepository;

        public EditUserProfileHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override EmptyResponse Handle(EditRequest<UserProfile> request)
        {
            _userRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
   }
}