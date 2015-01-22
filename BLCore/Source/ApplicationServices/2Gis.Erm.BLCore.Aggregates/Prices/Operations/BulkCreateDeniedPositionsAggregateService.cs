using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkCreateDeniedPositionsAggregateService : IBulkCreateDeniedPositionsAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateDeniedPositionsAggregateService(IIdentityProvider identityProvider,
                                                         IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                         IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(IEnumerable<DeniedPosition> deniedPositions, long priceId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                foreach (var deniedPosition in deniedPositions)
                {
                    _identityProvider.SetFor(deniedPosition);
                    deniedPosition.PriceId = priceId;

                    _deniedPositionGenericRepository.Add(deniedPosition);
                    operationScope.Added<DeniedPosition>(deniedPosition.Id);
                }

                var count = _deniedPositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}