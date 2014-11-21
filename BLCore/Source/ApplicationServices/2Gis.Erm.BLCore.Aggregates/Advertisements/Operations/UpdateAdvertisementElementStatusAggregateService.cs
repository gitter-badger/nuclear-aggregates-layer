using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public class UpdateAdvertisementElementStatusAggregateService : IAggregatePartRepository<Advertisement>,
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
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.AdvertisementElementStatus))
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