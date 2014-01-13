using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditTerritoryHandler : RequestHandler<EditRequest<Territory>, EmptyResponse>
    {
        private readonly IUserRepository _userRepository;

        public EditTerritoryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Territory> request)
        {
            var territory = request.Entity;

            _userRepository.CreateOrUpdate(territory);   

            return Response.Empty;
        }
    }
}