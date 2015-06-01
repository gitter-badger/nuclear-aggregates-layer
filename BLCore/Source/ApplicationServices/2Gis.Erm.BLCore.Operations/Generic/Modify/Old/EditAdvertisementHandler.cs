using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdvertisementHandler : RequestHandler<EditRequest<Advertisement>, EmptyResponse>
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IBulkCreateAdvertisementElementsForAdvertisementAggregateService _elementsForAdvertisementService;

        public EditAdvertisementHandler(
            IAdvertisementReadModel advertisementReadModel,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            IOperationScopeFactory scopeFactory,
            IAdvertisementRepository advertisementRepository,
            IBulkCreateAdvertisementElementsForAdvertisementAggregateService elementsForAdvertisementService)
        {
            _advertisementReadModel = advertisementReadModel;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
            _scopeFactory = scopeFactory;
            _advertisementRepository = advertisementRepository;
            _elementsForAdvertisementService = elementsForAdvertisementService;
        }

        protected override EmptyResponse Handle(EditRequest<Advertisement> request)
        {
            var advertisement = request.Entity;

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(advertisement))
            {
                var isNewAdvertisement = advertisement.IsNew();

                if (advertisement.FirmId.HasValue && !_advertisementRepository.IsAdvertisementTemplatePublished(advertisement.AdvertisementTemplateId))
                {
                    throw new BusinessLogicException(BLResources.CantEditAdvertisementWithUnpublishedTemplate);
                }

                if (!advertisement.FirmId.HasValue && _advertisementRepository.IsAdvertisementTemplatePublished(advertisement.AdvertisementTemplateId))
                {
                    throw new BusinessLogicException(BLResources.CanNotChangeDummyAdvertisementWithPublishedTemplate);
                }

                if (!isNewAdvertisement &&
                    !_advertisementRepository.IsAdvertisementTemplateTheSameInAdvertisementAndElements(request.Entity.AdvertisementTemplateId, request.Entity.Id))
                {
                    throw new BusinessLogicException(BLResources.CannotChangeAdvertisementSinceItsTemplateWasChanged);
                }

                // Инициализирует по необходимости идентификатор advertisement
                _advertisementRepository.CreateOrUpdate(advertisement);

                // создаём вместо с РМ дочерние элементы РМ
                if (isNewAdvertisement)
                {
                    var elementsToCreate = _advertisementReadModel.GetElementsToCreate(advertisement.AdvertisementTemplateId);
                    _elementsForAdvertisementService.Create(elementsToCreate, advertisement.Id, advertisement.OwnerCode);
                }

                // фиксируем факт косвенного влияния изменений на некоторые заказы
                var orderIds = _advertisementReadModel.GetDependedOrderIds(new[] { advertisement.Id });

                _registerOrderStateChangesOperationService.Changed(orderIds.Select(x => new OrderChangesDescriptor
                                                                                           {
                                                                                               OrderId = x,
                                                                                               ChangedAspects =
                                                                                                   new[]
                                                                                                       {
                                                                                                           OrderValidationRuleGroup
                                                                                                               .AdvertisementMaterialsValidation
                                                                                                       }
                                                                                           }));

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}

