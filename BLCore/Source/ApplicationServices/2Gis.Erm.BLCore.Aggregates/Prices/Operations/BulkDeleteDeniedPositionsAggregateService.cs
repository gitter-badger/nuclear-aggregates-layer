using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkDeleteDeniedPositionsAggregateService : IBulkDeleteDeniedPositionsAggregateService
    {
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkDeleteDeniedPositionsAggregateService(IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                         IOperationScopeFactory operationScopeFactory)
        {
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Delete(IEnumerable<DeniedPosition> deniedPositions)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    _deniedPositionGenericRepository.Delete(deniedPosition);
                    operationScope.Deleted(deniedPosition);
                }

                _deniedPositionGenericRepository.Save();

                operationScope.Complete();
            }
        }
    }
}