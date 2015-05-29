using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class CreateAssociatedPositionService : ICreateAggregateRepository<AssociatedPosition>, IAggregateRootService<Price>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<AssociatedPosition> _associatedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateAssociatedPositionService(IIdentityProvider identityProvider,
                                               IRepository<AssociatedPosition> associatedPositionGenericRepository,
                                               IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _associatedPositionGenericRepository = associatedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(AssociatedPosition entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, AssociatedPosition>())
            {
                _identityProvider.SetFor(entity);

                _associatedPositionGenericRepository.Add(entity);
                operationScope.Added<AssociatedPosition>(entity.Id);

                var count = _associatedPositionGenericRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}