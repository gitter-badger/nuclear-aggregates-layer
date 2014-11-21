using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.BranchOfficeOrganizationUnit;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.BranchOfficeOrganizationUnits
{
    public sealed class SetBranchOfficeOrganizationUnitAsPrimary : RequestHandler<SetBranchOfficeOrganizationUnitAsPrimaryRequest, EmptyResponse>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public SetBranchOfficeOrganizationUnitAsPrimary(IBranchOfficeRepository branchOfficeRepository, IOperationScopeFactory scopeFactory)
        {
            _branchOfficeRepository = branchOfficeRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(SetBranchOfficeOrganizationUnitAsPrimaryRequest request)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<SetBranchOfficeOrganizationUnitAsPrimaryIdentity>())
            {
                _branchOfficeRepository.SetPrimaryBranchOfficeOrganizationUnit(request.Id);
                scope.Complete();
            }

            return Response.Empty;
        }
    }
}