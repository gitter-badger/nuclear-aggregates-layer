using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

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

        public void Deactivate(IEnumerable<DeniedPosition> deniedPositions)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    deniedPosition.IsActive = false;

                    _deniedPositionGenericRepository.Update(deniedPosition);
                    operationScope.Updated(deniedPosition);
                }

                _deniedPositionGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}