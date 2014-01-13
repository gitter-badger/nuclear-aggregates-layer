using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.BranchOfficeOrganizationUnits
{
    public sealed class SetBranchOfficeOrganizationUnitAsPrimary : RequestHandler<SetBranchOfficeOrganizationUnitAsPrimaryRequest, EmptyResponse>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        
        public SetBranchOfficeOrganizationUnitAsPrimary(IBranchOfficeRepository branchOfficeRepository)
        {
            _branchOfficeRepository = branchOfficeRepository;
        }

        protected override EmptyResponse Handle(SetBranchOfficeOrganizationUnitAsPrimaryRequest request)
        {
            _branchOfficeRepository.SetPrimaryBranchOfficeOrganizationUnit(request.Id);
            return Response.Empty;
        }
    }
}