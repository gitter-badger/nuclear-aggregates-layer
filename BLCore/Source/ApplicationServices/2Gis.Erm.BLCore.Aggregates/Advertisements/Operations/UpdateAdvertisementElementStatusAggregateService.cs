using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public class UpdateAdvertisementElementStatusAggregateService : IAggregatePartService<Advertisement>,
                                                                    IUpdateAggregateRepository<AdvertisementElementStatus>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<AdvertisementElementStatus> _entityRepository;

        public UpdateAdvertisementElementStatusAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<AdvertisementElementStatus> entityRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _entityRepository = entityRepository;
        }

        public int Update(AdvertisementElementStatus entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, AdvertisementElementStatus>())
            {
                _entityRepository.Update(entity);
                operationScope.Updated<AdvertisementElementStatus>(entity.Id);

                var count = _entityRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}