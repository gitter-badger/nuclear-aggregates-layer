using DoubleGis.Erm.BLCore.API.Aggregates.Roles;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditRoleHandler : RequestHandler<EditRequest<Role>, EmptyResponse>
    {
        private readonly IRoleRepository _roleRepository;

        public EditRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Role> request)
        {
            _roleRepository.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}
