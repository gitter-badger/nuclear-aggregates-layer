using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdvertisementHandler : RequestHandler<EditRequest<Advertisement>, EmptyResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IOrderValidationInvalidator _orderValidationInvalidator;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAdvertisementRepository _advertisementRepository;

        public EditAdvertisementHandler(
            IAdvertisementReadModel advertisementReadModel,
            IUnitOfWork unitOfWork,
            IOrderValidationInvalidator orderValidationInvalidator,
            IOperationScopeFactory scopeFactory,
            IAdvertisementRepository advertisementRepository)
        {
            _advertisementReadModel = advertisementReadModel;
            _orderValidationInvalidator = orderValidationInvalidator;
            _scopeFactory = scopeFactory;
            _unitOfWork = unitOfWork;
            _advertisementRepository = advertisementRepository;
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

                using (var uowScope = _unitOfWork.CreateScope())
                {
                    var advertisementRepository = uowScope.CreateRepository<IAdvertisementRepository>();

                    // Инициализирует по необходимости идентификатор advertisement
                    advertisementRepository.CreateOrUpdate(advertisement);

                    // создаём вместо с РМ дочерние элементы РМ
                    if (isNewAdvertisement)
                    {
                        advertisementRepository.AddAdvertisementsElementsFromAdvertisement(advertisement);
                    }

                    // сбрасываем кеш проверок заказов
                    var orderIds = _advertisementReadModel.GetDependedOrderIds(new[] { advertisement.Id });
                    _orderValidationInvalidator.Invalidate(orderIds, OrderValidationRuleGroup.AdvertisementMaterialsValidation);

                    uowScope.Complete();
                    operationScope.Complete();
                }

                return Response.Empty;
            }
        }
    }
}
