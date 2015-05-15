using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Deactivate
{
    public class DeactivateBranchOfficeOrganizationUnitService : IDeactivateGenericEntityService<BranchOfficeOrganizationUnit>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeactivateBranchOfficeOrganizationUnitService(IBranchOfficeRepository branchOfficeRepository, IOperationScopeFactory scopeFactory)
        {
            _branchOfficeRepository = branchOfficeRepository;
            _scopeFactory = scopeFactory;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, BranchOfficeOrganizationUnit>())
            {
                var deactivateAggregateRepository = (IDeactivateAggregateRepository<BranchOfficeOrganizationUnit>)_branchOfficeRepository;
                deactivateAggregateRepository.Deactivate(entityId);

                scope.Complete();

                return null;
            }
        }
    }
}