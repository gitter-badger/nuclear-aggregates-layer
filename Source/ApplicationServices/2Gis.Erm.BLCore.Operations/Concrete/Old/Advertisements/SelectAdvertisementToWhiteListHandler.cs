using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Advertisements
{
    public sealed class SelectAdvertisementToWhiteListHandler : RequestHandler<SelectAdvertisementToWhiteListRequest, EmptyResponse>
    {
        private readonly IOrderValidationResultsResetter _orderValidationResultsResetter;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IFirmRepository _firmRepository;

        public SelectAdvertisementToWhiteListHandler(IOrderValidationResultsResetter orderValidationResultsResetter,
            IOperationScopeFactory scopeFactory, 
            IAdvertisementRepository advertisementRepository, 
            IFirmRepository firmRepository)
        {
            _orderValidationResultsResetter = orderValidationResultsResetter;
            _scopeFactory = scopeFactory;
            _advertisementRepository = advertisementRepository;
            _firmRepository = firmRepository;
        }

        protected override EmptyResponse Handle(SelectAdvertisementToWhiteListRequest request)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<SelectAdvertisementToWhitelistIdentity>())
            {
                _advertisementRepository.SelectToWhiteList(request.FirmId, request.AdvertisementId);

                // сбрасываем кеш проверок заказов
                var advertisementIds = _firmRepository.GetAdvertisementIds(request.FirmId);
                var orderIds = _advertisementRepository.GetDependedOrderIds(advertisementIds);

                foreach (var orderId in orderIds)
                {
                    _orderValidationResultsResetter.SetGroupAsInvalid(orderId, OrderValidationRuleGroup.AdvertisementMaterialsValidation);
                }

                operationScope
                    .Updated<Advertisement>(request.AdvertisementId)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}