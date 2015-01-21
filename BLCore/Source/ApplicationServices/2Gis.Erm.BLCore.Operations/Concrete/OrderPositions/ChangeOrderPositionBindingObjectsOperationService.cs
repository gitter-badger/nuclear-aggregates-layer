using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.OrderPositions;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    public class ChangeOrderPositionBindingObjectsOperationService : IChangeOrderPositionBindingObjectsOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICanChangeOrderPositionBindingObjectsDetector _canChangeOrderPositionBindingObjectsDetector;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;
        private readonly IOrderReplaceOrderPositionAdvertisementLinksAggregateService _replaceOrderPositionAdvertisementLinksAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChangeOrderPositionBindingObjectsOperationService(
            IOrderReadModel orderReadModel,
            ICanChangeOrderPositionBindingObjectsDetector canChangeOrderPositionBindingObjectsDetector,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            IOrderReplaceOrderPositionAdvertisementLinksAggregateService replaceOrderPositionAdvertisementLinksAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _orderReadModel = orderReadModel;
            _canChangeOrderPositionBindingObjectsDetector = canChangeOrderPositionBindingObjectsDetector;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
            _replaceOrderPositionAdvertisementLinksAggregateService = replaceOrderPositionAdvertisementLinksAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Change(long orderPositionId, IReadOnlyList<AdvertisementDescriptor> advertisementLinkDescriptors)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ChangeOrderPositionBindingObjectsIdentity>())
            {
                var orderPositionAdvertisementLinksInfo = _orderReadModel.GetOrderPositionAdvertisementLinksInfo(orderPositionId);

                string report;
                if (!_canChangeOrderPositionBindingObjectsDetector.CanChange(
                    orderPositionAdvertisementLinksInfo.OrderWorkflowState, 
                    orderPositionAdvertisementLinksInfo.BindingType,
                    false,
                    orderPositionAdvertisementLinksInfo.AdvertisementLinks.Count(),
                    advertisementLinkDescriptors.Count,
                    out report))
                {
                    throw new BusinessLogicException(report);
                }

                _replaceOrderPositionAdvertisementLinksAggregateService.Replace(
                    orderPositionAdvertisementLinksInfo.AdvertisementLinks,
                    advertisementLinkDescriptors.Select(descriptor => new AdvertisementLinkDescriptor
                                                                          {
                                                                              OrderPositionId = orderPositionId,
                                                                              AdvertisementId = descriptor.AdvertisementId,
                                                                              CategoryId = descriptor.CategoryId,
                                                                              FirmAddressId = descriptor.FirmAddressId,
                                                                              PositionId = descriptor.PositionId,
                                                                              ThemeId = descriptor.ThemeId
                                                                          }));

                _registerOrderStateChangesOperationService.Changed(new[]
                                                                       {
                                                                           new OrderChangesDescriptor
                                                                               {
                                                                                   OrderId = orderPositionAdvertisementLinksInfo.OrderId,
                                                                                   ChangedAspects = new[]
                                                                                                        {
                                                                                                            OrderValidationRuleGroup.Generic,
                                                                                                            OrderValidationRuleGroup.SalesModelValidation,
                                                                                                            OrderValidationRuleGroup.AdvertisementMaterialsValidation,
                                                                                                            OrderValidationRuleGroup.ADPositionsValidation
                                                                                                        }
                                                                               }
                                                                       });

                scope.Complete();
            }
        }
    }
}
