using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteAdvertisementOperationService : IDeleteGenericEntityService<Advertisement>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteAdvertisementOperationService(IAdvertisementRepository advertisementRepository, IOperationScopeFactory scopeFactory)
        {
            _advertisementRepository = advertisementRepository;
            _scopeFactory = scopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Advertisement>())
            {
                var deleteAggregateRepository = (IDeleteAggregateRepository<Advertisement>)_advertisementRepository;
                deleteAggregateRepository.Delete(entityId);

                operationScope
                    .Deleted<Advertisement>(entityId)
                    .Complete();
            }

            return null;
        }

        // not used
        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}