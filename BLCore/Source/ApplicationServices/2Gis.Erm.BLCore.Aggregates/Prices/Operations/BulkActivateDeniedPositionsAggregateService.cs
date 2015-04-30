using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkActivateDeniedPositionsAggregateService : IBulkActivateDeniedPositionsAggregateService
    {
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkActivateDeniedPositionsAggregateService(IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                           IOperationScopeFactory operationScopeFactory)
        {
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Activate(IEnumerable<DeniedPosition> deniedPositions)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    deniedPosition.IsActive = true;
                    _deniedPositionGenericRepository.Update(deniedPosition);
                    operationScope.Updated<DeniedPosition>(deniedPosition.Id);
                }

                var count = _deniedPositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}