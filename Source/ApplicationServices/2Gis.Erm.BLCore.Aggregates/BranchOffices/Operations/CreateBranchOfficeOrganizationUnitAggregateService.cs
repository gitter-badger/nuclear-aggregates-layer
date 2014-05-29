using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class CreateBranchOfficeOrganizationUnitAggregateService : IAggregateRootRepository<BranchOffice>, ICreateAggregateRepository<BranchOfficeOrganizationUnit>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;

        public CreateBranchOfficeOrganizationUnitAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
        }

        public int Create(BranchOfficeOrganizationUnit entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, BranchOfficeOrganizationUnit>())
            {
                foreach (var part in entity.Parts)
                {
                    _identityProvider.SetFor(part);
                }

                _branchOfficeOrgUnitSecureRepository.Add(entity);
                operationScope.Added<BranchOfficeOrganizationUnit>(entity.Id);

                var count = _branchOfficeOrgUnitSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}