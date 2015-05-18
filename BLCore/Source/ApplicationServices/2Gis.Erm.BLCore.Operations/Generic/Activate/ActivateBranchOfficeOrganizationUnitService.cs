﻿using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateBranchOfficeOrganizationUnitService : IActivateGenericEntityService<BranchOfficeOrganizationUnit>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ActivateBranchOfficeOrganizationUnitService(IBranchOfficeRepository branchOfficeRepository, IOperationScopeFactory scopeFactory)
        {
            _branchOfficeRepository = branchOfficeRepository;
            _scopeFactory = scopeFactory;
        }

        public int Activate(long entityId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ActivateIdentity, BranchOfficeOrganizationUnit>())
            {
                var activateAggregateRepository = (IActivateAggregateRepository<BranchOfficeOrganizationUnit>)_branchOfficeRepository;
                var result = activateAggregateRepository.Activate(entityId);

                scope.Complete();

                return result;
            }
        }
    }
}