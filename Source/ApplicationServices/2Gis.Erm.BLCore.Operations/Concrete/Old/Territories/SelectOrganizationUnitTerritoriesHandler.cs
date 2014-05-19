using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Territories
{
    public sealed class SelectOrganizationUnitTerritoriesHandler : RequestHandler<SelectOrganizationUnitTerritoriesRequest, SelectOrganizationUnitTerritoriesResponse>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public SelectOrganizationUnitTerritoriesHandler(IBranchOfficeRepository branchOfficeRepository)
        {
            _branchOfficeRepository = branchOfficeRepository;
        }

        protected override SelectOrganizationUnitTerritoriesResponse Handle(SelectOrganizationUnitTerritoriesRequest request)
        {
            return new SelectOrganizationUnitTerritoriesResponse
                {
                    TerritoryIds = _branchOfficeRepository.GetOrganizationUnitTerritories(request.OrganizationUnitId)
                };
        }
    }
}