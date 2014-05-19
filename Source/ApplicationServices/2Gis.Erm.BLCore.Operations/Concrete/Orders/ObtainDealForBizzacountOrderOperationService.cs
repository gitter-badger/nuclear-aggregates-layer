using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations\Operations\Concrete\Orders
    public class ObtainDealForBizzacountOrderOperationService : IObtainDealForBizzacountOrderOperationService
    {
        private readonly IDealRepository _dealRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IGenerateDealNameService _generateDealNameService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IClientDealSelectionService _clientDealSelectionService;
        private readonly IGetBaseCurrencyService _currencyService;

        public ObtainDealForBizzacountOrderOperationService(
            IDealRepository dealRepository,
            IIdentityProvider identityProvider,
            IGenerateDealNameService generateDealNameService,
            IOperationScopeFactory operationScopeFactory,
            IClientDealSelectionService clientDealSelectionService,
            IGetBaseCurrencyService currencyService)
        {
            _dealRepository = dealRepository;
            _identityProvider = identityProvider;
            _generateDealNameService = generateDealNameService;
            _operationScopeFactory = operationScopeFactory;
            _clientDealSelectionService = clientDealSelectionService;
            _currencyService = currencyService;
        }

        /// <summary>
        /// ���������� ������.
        /// 
        /// ���� � ������� ���� � ������ ���� ������� ������, ���������� �.
        /// �����, ���� ������ ��� ��� ���������, �� � � ����� �� ��� ������� �� firmOwnerCode, �� �������� ����� ������ � ��������� clientOwnerCode
        /// </summary>
        /// <param name="clientId">������������� ������� ����� ������</param>
        /// <param name="clientOwnerCode">������� ������� ����� ������</param>
        /// <returns>�������������� ������������ ��� ��������� ������. �� null.</returns>
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
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.Deal))
            {
                var newDeal = new Deal
                {
                    Name = _generateDealNameService.GenerateDealName(clientId),
                    MainFirmId = _clientDealSelectionService.GetMainFirmId(clientId),
                    ClientId = clientId,
                    CurrencyId = _currencyService.GetBaseCurrency().Id,
                    StartReason = (int)ReasonForNewDeal.Bizaccount,
                    DealStage = (int)DealStage.OrderFormed,
                    OwnerCode = ownerCode,
                    IsActive = true,
                    IsDeleted = false
                };

                _identityProvider.SetFor(newDeal);
                _dealRepository.Add(newDeal);

                operationScope
                    .Added<Deal>(newDeal.Id)
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