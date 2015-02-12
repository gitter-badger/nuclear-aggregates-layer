using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.OrderPositions;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    public class ReplaceOrderPositionAdvertisementLinksOperationService : IReplaceOrderPositionAdvertisementLinksOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IValidateOrderPositionAdvertisementsOperationService _validateOrderPositionAdvertisementsOperationService;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;
        private readonly IOrderReplaceOrderPositionAdvertisementLinksAggregateService _replaceOrderPositionAdvertisementLinksAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ReplaceOrderPositionAdvertisementLinksOperationService(
            IOrderReadModel orderReadModel,
            IValidateOrderPositionAdvertisementsOperationService validateOrderPositionAdvertisementsOperationService,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            IOrderReplaceOrderPositionAdvertisementLinksAggregateService replaceOrderPositionAdvertisementLinksAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _orderReadModel = orderReadModel;
            _validateOrderPositionAdvertisementsOperationService = validateOrderPositionAdvertisementsOperationService;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
            _replaceOrderPositionAdvertisementLinksAggregateService = replaceOrderPositionAdvertisementLinksAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Replace(long orderPositionId, IReadOnlyList<AdvertisementDescriptor> advertisementLinkDescriptors)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ReplaceOrderPositionAdvertisementLinksIdentity>())
            {
                var orderPositionAdvertisementLinksInfo = _orderReadModel.GetOrderPositionAdvertisementLinksInfo(orderPositionId);

                if (orderPositionAdvertisementLinksInfo.OrderWorkflowState != OrderState.OnRegistration
                    && IsChangedOrderPositionAdvertisementsLinksInLockedOrder(
                            orderPositionAdvertisementLinksInfo.AdvertisementLinks,
                            advertisementLinkDescriptors))
                {
                    // ошибка если как-то смогли изменить позиции у заблокированного заказа
                    throw new BusinessLogicException(BLResources.ChangingAdvertisementLinksIsDeniedWhileOrderIsLocked);
                }

                // повторяю прежнюю логику. По-хорошему все ошибки можно показать в окошечке. Сейчас этого не делаем, т.к.надо тестировать и релизить.
                var firstError = _validateOrderPositionAdvertisementsOperationService.Validate(orderPositionId, advertisementLinkDescriptors).FirstOrDefault();
                if (firstError != null)
                {
                    throw new BusinessLogicException(firstError.ErrorMessage);
                }

                _replaceOrderPositionAdvertisementLinksAggregateService.Replace(
                    orderPositionAdvertisementLinksInfo.AdvertisementLinks, 
                    advertisementLinkDescriptors.Select(descriptor => 
                        new AdvertisementLinkDescriptor
                            {
                                AdvertisementId = descriptor.AdvertisementId,
                                CategoryId = descriptor.CategoryId,
                                FirmAddressId = descriptor.FirmAddressId,
                                OrderPositionId = orderPositionId,
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
                                                                                                        OrderValidationRuleGroup.SalesModelValidation,
                                                                                                        OrderValidationRuleGroup.AdvertisementMaterialsValidation
                                                                                                    }
                                                                           }
                                                                   });

                scope.Updated<OrderPosition>(orderPositionId)
                     .Complete();
            }
        }

        private static bool IsChangedOrderPositionAdvertisementsLinksInLockedOrder(
            IEnumerable<OrderPositionAdvertisement> oldAdvertisementsLinks,
            IEnumerable<AdvertisementDescriptor> newAdvertisementsLinks)
        {
            // поэлементная сортировка 
            var oldAdvertisementsLinksOrdered = oldAdvertisementsLinks
                .OrderBy(x => x.PositionId)
                .ThenBy(x => x.FirmAddressId)
                .ThenBy(x => x.CategoryId)
                .ToArray();

            var newAdvertisementsLinksOrdered = newAdvertisementsLinks
                .OrderBy(x => x.PositionId)
                .ThenBy(x => x.FirmAddressId)
                .ThenBy(x => x.CategoryId)
                .ToArray();

            if (oldAdvertisementsLinksOrdered.Length != newAdvertisementsLinksOrdered.Length)
            {
                return true;
            }

            for (var i = 0; i < newAdvertisementsLinksOrdered.Length; i++)
            {
                var oldAdvertisementsLink = oldAdvertisementsLinksOrdered[i];
                var newAdvertisementsLink = newAdvertisementsLinksOrdered[i];

                if (newAdvertisementsLink.PositionId != oldAdvertisementsLink.PositionId ||
                    newAdvertisementsLink.FirmAddressId != oldAdvertisementsLink.FirmAddressId ||
                    newAdvertisementsLink.CategoryId != oldAdvertisementsLink.CategoryId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
