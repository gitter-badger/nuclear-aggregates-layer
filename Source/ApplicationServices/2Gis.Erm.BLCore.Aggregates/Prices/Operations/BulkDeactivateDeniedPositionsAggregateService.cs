﻿using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkDeactivateDeniedPositionsAggregateService : IBulkDeactivateDeniedPositionsAggregateService
    {
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkDeactivateDeniedPositionsAggregateService(IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                             IOperationScopeFactory operationScopeFactory)
        {
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Deactivate(IEnumerable<DeniedPosition> deniedPositions)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    _deniedPositionGenericRepository.Delete(deniedPosition);
                    operationScope.Deleted<DeniedPosition>(deniedPosition.Id);
                }

                var count = _deniedPositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}