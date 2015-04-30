using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public class CreateAdvertisementElementDenialReasonAggregateService : IAggregatePartRepository<Advertisement>,
                                                                          ICreateAggregateRepository<AdvertisementElementDenialReason>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<AdvertisementElementDenialReason> _entityRepository;
        private readonly IIdentityProvider _identityProvider;

        public CreateAdvertisementElementDenialReasonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<AdvertisementElementDenialReason> entityRepository,
            IIdentityProvider identityProvider)
        {
            _operationScopeFactory = operationScopeFactory;
            _entityRepository = entityRepository;
            _identityProvider = identityProvider;
        }

        public int Create(AdvertisementElementDenialReason entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, AdvertisementElementDenialReason>())
            {
                _identityProvider.SetFor(entity);
                _entityRepository.Add(entity);
                operationScope.Added<AdvertisementElementDenialReason>(entity.Id);

                var count = _entityRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}