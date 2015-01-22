using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class ObtainDealForBizzacountOrderOperationService : IObtainDealForBizzacountOrderOperationService
    {
        private readonly IGenerateDealNameService _generateDealNameService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IClientDealSelectionService _clientDealSelectionService;
        private readonly IGetBaseCurrencyService _currencyService;
        private readonly ICreateDealAggregateService _createDealAggregateService;

        public ObtainDealForBizzacountOrderOperationService(
            IGenerateDealNameService generateDealNameService,
            IOperationScopeFactory operationScopeFactory,
            IClientDealSelectionService clientDealSelectionService,
            IGetBaseCurrencyService currencyService,
            ICreateDealAggregateService createDealAggregateService)
        {
            _generateDealNameService = generateDealNameService;
            _operationScopeFactory = operationScopeFactory;
            _clientDealSelectionService = clientDealSelectionService;
            _currencyService = currencyService;
            _createDealAggregateService = createDealAggregateService;
        }

        /// <summary>
        /// Возвращает сделку.
        /// 
        /// Если у клиента одна и только одна окрытая сделка, возвращает её.
        /// Иначе, если сделок нет или несколько, но и у одной из них куратор не firmOwnerCode, то создаётся новая сделка с куратором clientOwnerCode
        /// </summary>
        /// <param name="clientId">Идентификатор клиента фирмы заявки</param>
        /// <param name="clientOwnerCode">Куратор клиента фирмы заявки</param>
        /// <returns>Иденетификатор существующей или созданной сделки. Не null.</returns>
        public ObtainDealForBizzacountOrderResult CreateDealForClient(long clientId, long clientOwnerCode)
        {
            return ObtainDeal(() => CreateDealForClientInternal(clientId, clientOwnerCode));
        }

        public ObtainDealForBizzacountOrderResult ObtainDealForOrder(long orderId, long ownerCode)
        {
            return ObtainDeal(() => GetDealForOrder(orderId) ?? CreateDealForOrder(orderId, ownerCode));
        }

        private ObtainDealForBizzacountOrderResult ObtainDeal(Func<Deal> dealSelector)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<ObtainDealForBizaccountOrderIdentity>())
            {
                var dealForOrder = dealSelector();

                scope.Complete();

                return new ObtainDealForBizzacountOrderResult
                           {
                               DealId = dealForOrder.Id,
                               DealOwnerCode = dealForOrder.OwnerCode,
                               DealName = dealForOrder.Name,
                           };
            }
        }

        private Deal CreateDealForOrder(long orderId, long ownerCode)
        {
            var clientId = _clientDealSelectionService.GetOrderClientId(orderId);

            if (clientId == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ClientOfFirmOrderMissingTemplate, orderId));
            }

            return CreateDealForClientInternal(clientId.Value, ownerCode);
        }

        private Deal CreateDealForClientInternal(long clientId, long ownerCode)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Deal>())
            {
                var newDeal = new Deal
                                  {
                                      Name = _generateDealNameService.GenerateDealName(clientId),
                                      MainFirmId = _clientDealSelectionService.GetMainFirmId(clientId),
                                      ClientId = clientId,
                                      CurrencyId = _currencyService.GetBaseCurrency().Id,
                                      StartReason = ReasonForNewDeal.Bizaccount,
                                      DealStage = DealStage.OrderFormed,
                                      OwnerCode = ownerCode,
                                      IsActive = true,
                                      IsDeleted = false
                                  };

                _createDealAggregateService.Create(newDeal);

                operationScope
                    .Added(newDeal)
                    .Complete();

                return newDeal;
            }
        }

        private Deal GetDealForOrder(long orderId)
        {
            var deal = _clientDealSelectionService.GetDealForOrder(orderId);

            return deal != null && deal.CloseDate == null ? deal : null;
        }
    }
}