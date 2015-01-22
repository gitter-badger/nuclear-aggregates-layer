using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class CreateDeniedPositionAggregateService : ICreateAggregateRepository<DeniedPosition>, IAggregateRootRepository<Price>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<DeniedPosition> _deniedPositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateDeniedPositionAggregateService(IIdentityProvider identityProvider,
                                                    IRepository<DeniedPosition> deniedPositionGenericRepository,
                                                    IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _deniedPositionGenericRepository = deniedPositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Create(DeniedPosition entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                _identityProvider.SetFor(entity);

                _deniedPositionGenericRepository.Add(entity);
                operationScope.Added<DeniedPosition>(entity.Id);

                var count = _deniedPositionGenericRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}