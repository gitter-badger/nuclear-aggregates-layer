using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdvertisementTemplateHandler : RequestHandler<EditRequest<AdvertisementTemplate>, EmptyResponse>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public EditAdvertisementTemplateHandler(
            IAdvertisementRepository advertisementRepository,
            IPublicService publicService,
            IOperationScopeFactory operationScopeFactory)
        {
            _advertisementRepository = advertisementRepository;
            _publicService = publicService;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<AdvertisementTemplate> request)
        {
                var advertisementTemplate = request.Entity;

            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(advertisementTemplate))
            {
                var isNew = advertisementTemplate.IsNew();
                if (!isNew && _advertisementRepository.IsAdvertisementTemplatePublished(advertisementTemplate.Id))
                {
                    throw new BusinessLogicException(BLResources.CanNotChangePublishedAdvertisementTemplate);
                }

                if (isNew)
                {
                        _advertisementRepository.CreateOrUpdate(advertisementTemplate);

                        var advertisement = new Advertisement
                            {
                                AdvertisementTemplateId = advertisementTemplate.Id,
                                Name = BLResources.DummyValue
                            };

                        _publicService.Handle(new EditRequest<Advertisement> { Entity = advertisement });
                        advertisementTemplate.DummyAdvertisementId = advertisement.Id;
                        _advertisementRepository.CreateOrUpdate(advertisementTemplate);

                        // fixme {a.rechkalov}: правильное OperationIdentity
                    operationScope.Added<AdvertisementTemplate>(advertisementTemplate.Id)
                                  .Updated<AdvertisementTemplate>(advertisementTemplate.Id);
                }
                else
                {
                        _advertisementRepository.CreateOrUpdate(advertisementTemplate);

                        // fixme {a.rechkalov}: правильное OperationIdentity
                        operationScope.Updated<AdvertisementTemplate>(advertisementTemplate.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}