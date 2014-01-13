using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditOrganizationUnitHandler : RequestHandler<EditRequest<OrganizationUnit>, EmptyResponse>
    {
        private readonly IUserRepository _userRepository;

        public EditOrganizationUnitHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override EmptyResponse Handle(EditRequest<OrganizationUnit> request)
        {
            var organizationUnit = request.Entity;
            _userRepository.CreateOrUpdate(organizationUnit);
            return Response.Empty;
        }
    }
}