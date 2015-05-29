using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public class DeleteAdvertisementElementDenialReasonAggregateService : IAggregatePartService<Advertisement>,
                                                                          IDeleteAggregateRepository<AdvertisementElementDenialReason>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<AdvertisementElementDenialReason> _entityRepository;
        private readonly IAdvertisementReadModel _advertisementReadModel;

        public DeleteAdvertisementElementDenialReasonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<AdvertisementElementDenialReason> entityRepository,
            IAdvertisementReadModel advertisementReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _entityRepository = entityRepository;
            _advertisementReadModel = advertisementReadModel;
        }

        public int Delete(AdvertisementElementDenialReason entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, AdvertisementElementDenialReason>())
            {
                _entityRepository.Delete(entity);
                operationScope.Deleted<AdvertisementElementDenialReason>(entity.Id);

                var count = _entityRepository.Save();

                operationScope.Complete();
                return count;
            }
        }

        public int Delete(long entityId)
        {
            return Delete(_advertisementReadModel.GetAdvertisementElementDenialReason(entityId));
        }
    }
}