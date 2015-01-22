using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public class CreateHotClientRequestAggregateService : ICreateHotClientRequestAggregateService
    {
        private readonly IRepository<HotClientRequest> _hotClientRequestGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public CreateHotClientRequestAggregateService(IOperationScopeFactory scopeFactory,
                                                      IIdentityProvider identityProvider,
                                                      IRepository<HotClientRequest> hotClientRequestGenericRepository)
        {
            _scopeFactory = scopeFactory;
            _identityProvider = identityProvider;
            _hotClientRequestGenericRepository = hotClientRequestGenericRepository;
        }

        public void Create(HotClientRequest hotClientRequest)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, HotClientRequest>())
            {
                _identityProvider.SetFor(hotClientRequest);
                _hotClientRequestGenericRepository.Add(hotClientRequest);

                _hotClientRequestGenericRepository.Save();

                operationScope.Added(hotClientRequest)
                              .Complete();
            }
        }
    }
}