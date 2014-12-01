using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
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
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IFirmRepository _firmRepository;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;

        public SelectAdvertisementToWhiteListHandler(
            IAdvertisementReadModel advertisementReadModel,
            IAdvertisementRepository advertisementRepository, 
            IFirmRepository firmRepository,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            IOperationScopeFactory scopeFactory)
        {
            _advertisementReadModel = advertisementReadModel;
            _scopeFactory = scopeFactory;
            _advertisementRepository = advertisementRepository;
            _firmRepository = firmRepository;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
        }

        protected override EmptyResponse Handle(SelectAdvertisementToWhiteListRequest request)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<SelectAdvertisementToWhitelistIdentity>())
            {
                _advertisementRepository.SelectToWhiteList(request.FirmId, request.AdvertisementId);

                // сбрасываем кеш проверок заказов
                var advertisementIds = _firmRepository.GetAdvertisementIds(request.FirmId);
                var orderIds = _advertisementReadModel.GetDependedOrderIds(advertisementIds);

                _registerOrderStateChangesOperationService.Changed(orderIds.Select(x => new OrderChangesDescriptor
                                                                                            {
                                                                                                OrderId = x,
                                                                                                ChangedAspects =
                                                                                                    new[]
                                                                                                        {
                                                                                                            OrderValidationRuleGroup.AdvertisementMaterialsValidation
                                                                                                        }
                                                                                            }));

                operationScope
                    .Updated<Advertisement>(request.AdvertisementId)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}