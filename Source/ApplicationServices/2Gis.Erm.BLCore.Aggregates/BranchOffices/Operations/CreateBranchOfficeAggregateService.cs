﻿using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class CreateBranchOfficeAggregateService : IAggregateRootRepository<BranchOffice>, ICreateAggregateRepository<BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;

        public CreateBranchOfficeAggregateService(IOperationScopeFactory operationScopeFactory,
                                                  IRepository<BranchOffice> branchOfficeRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
        }

        public int Create(BranchOffice entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.BranchOffice))
            {
                _branchOfficeRepository.Add(entity);
                operationScope.Added<BranchOffice>(entity.Id);
                
                var count = _branchOfficeRepository.Save();
                
                operationScope.Complete();

                return count;
            }
        }
    }
}
