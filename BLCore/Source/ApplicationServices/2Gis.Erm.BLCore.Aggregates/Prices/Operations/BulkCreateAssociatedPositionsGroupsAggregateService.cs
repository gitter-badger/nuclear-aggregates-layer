using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class BulkCreateAssociatedPositionsGroupsAggregateService : IBulkCreateAssociatedPositionsGroupsAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<AssociatedPositionsGroup> _associatedPositionsGroupGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public BulkCreateAssociatedPositionsGroupsAggregateService(IIdentityProvider identityProvider,
                                                                   IRepository<AssociatedPositionsGroup> associatedPositionsGroupGenericRepository,
                                                                   IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _associatedPositionsGroupGenericRepository = associatedPositionsGroupGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(IEnumerable<AssociatedPositionsGroup> associatedPositionsGroups, long pricePositionId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, AssociatedPositionsGroup>())
            {
                foreach (var associatedPositionsGroup in associatedPositionsGroups)
                {
                    _identityProvider.SetFor(associatedPositionsGroup);
                    associatedPositionsGroup.PricePositionId = pricePositionId;

                    _associatedPositionsGroupGenericRepository.Add(associatedPositionsGroup);
                    operationScope.Added<AssociatedPositionsGroup>(associatedPositionsGroup.Id);
                }

                var count = _associatedPositionsGroupGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}