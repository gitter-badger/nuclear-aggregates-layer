using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class CreateFirmContactAggregateService : IAggregatePartRepository<Firm>, ICreateAggregateRepository<FirmContact>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<FirmContact> _firmContactRepository;
        private readonly IIdentityProvider _identityProvider;

        public CreateFirmContactAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<FirmContact> firmContactRepository,
            IIdentityProvider identityProvider)
        {
            _operationScopeFactory = operationScopeFactory;
            _firmContactRepository = firmContactRepository;
            _identityProvider = identityProvider;
        }

        public int Create(FirmContact entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, FirmContact>())
            {
                _identityProvider.SetFor(entity);
                _firmContactRepository.Add(entity);
                operationScope.Added<FirmContact>(entity.Id);

                var count = _firmContactRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}