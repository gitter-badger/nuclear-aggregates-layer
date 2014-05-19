using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditDepartmentHandler : RequestHandler<EditRequest<Department>, EmptyResponse>
    {
        private readonly IUserRepository _userRepository;

        public EditDepartmentHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override EmptyResponse Handle(EditRequest<Department> request)
        {
            _userRepository.CreateOrUpdate(request.Entity);

            return Response.Empty;
        }
    }
}
