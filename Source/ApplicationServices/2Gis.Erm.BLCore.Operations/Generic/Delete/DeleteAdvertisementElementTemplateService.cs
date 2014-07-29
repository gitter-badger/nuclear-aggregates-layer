using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteAdvertisementElementTemplateService : IDeleteGenericEntityService<AdvertisementElementTemplate>
    {
        private readonly IAdvertisementRepository _advertisementRepository;

        public DeleteAdvertisementElementTemplateService(IAdvertisementRepository advertisementRepository)
        {
            _advertisementRepository = advertisementRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var deleteAggregateRepository = (IDeleteAggregateRepository<AdvertisementElementTemplate>)_advertisementRepository;
            deleteAggregateRepository.Delete(entityId);
            return null;
        }

        // not used
        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return null;
        }
    }
}