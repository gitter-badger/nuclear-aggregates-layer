using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkCreateAssociatedPositionsAggregateService : IBulkCreateAssociatedPositionsAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<AssociatedPosition> _associatedPositionsGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateAssociatedPositionsAggregateService(IIdentityProvider identityProvider,
                                                             IRepository<AssociatedPosition> associatedPositionsGenericRepository,
                                                             IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _associatedPositionsGenericRepository = associatedPositionsGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(IEnumerable<AssociatedPosition> associatedPositions, long associatedPositionsGroupId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, AssociatedPosition>())
            {
                foreach (var associatedPosition in associatedPositions)
                {
                    _identityProvider.SetFor(associatedPosition);
                    associatedPosition.AssociatedPositionsGroupId = associatedPositionsGroupId;

                    _associatedPositionsGenericRepository.Add(associatedPosition);
                    operationScope.Added<AssociatedPosition>(associatedPosition.Id);
                }

                var count = _associatedPositionsGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}