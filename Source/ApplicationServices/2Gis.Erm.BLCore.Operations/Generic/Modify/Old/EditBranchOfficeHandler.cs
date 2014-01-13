using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditBranchOfficeHandler : RequestHandler<EditRequest<BranchOffice>, EmptyResponse>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public EditBranchOfficeHandler(IBranchOfficeRepository branchOfficeRepository)
        {
            _branchOfficeRepository = branchOfficeRepository;
        }

        protected override EmptyResponse Handle(EditRequest<BranchOffice> request)
        {
            var office = request.Entity;
            _branchOfficeRepository.CreateOrUpdate(office);
            return Response.Empty;
        }
    }
}
