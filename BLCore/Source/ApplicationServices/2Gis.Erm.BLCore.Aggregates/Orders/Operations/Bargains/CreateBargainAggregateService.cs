using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Bargains
{
    public class CreateBargainAggregateService : IAggregateRootRepository<Order>, ICreateAggregateRepository<Bargain>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Bargain> _entitySecureRepository;

        public CreateBargainAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Bargain> entitySecureRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _entitySecureRepository = entitySecureRepository;
        }

        public int Create(Bargain entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Bargain>())
            {
                _identityProvider.SetFor(entity);
                _entitySecureRepository.Add(entity);
                operationScope.Added<Bargain>(entity.Id);

                var count = _entitySecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}