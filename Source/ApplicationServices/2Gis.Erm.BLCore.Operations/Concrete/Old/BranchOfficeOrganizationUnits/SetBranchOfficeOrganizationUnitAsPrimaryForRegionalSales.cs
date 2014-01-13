using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.BranchOfficeOrganizationUnits
{
    public sealed class SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSales : RequestHandler<SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest, EmptyResponse>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSales(IBranchOfficeRepository branchOfficeRepository)
        {
            _branchOfficeRepository = branchOfficeRepository;
        }

        protected override EmptyResponse Handle(SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest request)
        {
            _branchOfficeRepository.SetPrimaryForRegionalSalesBranchOfficeOrganizationUnit(request.Id);
            return Response.Empty;
        }
    }
}