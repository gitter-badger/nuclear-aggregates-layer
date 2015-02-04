using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderActualizeOrdersAmoutDuringWithdrawalAggregateService : IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService
    {
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IRepository<Order> _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public OrderActualizeOrdersAmoutDuringWithdrawalAggregateService(
            IBusinessModelSettings businessModelSettings,
            IRepository<Order> orderRepository, 
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _businessModelSettings = businessModelSettings;
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public IReadOnlyDictionary<long, Order> Actualize(IEnumerable<ActualizeOrdersDto> orders, bool isWithdrawalReverting)
        {
            var processedOrders = new Dictionary<long, Order>();

            _logger.InfoFormat("Starting actualizing order amount info during withdrawal process. Is reverting: {0}", isWithdrawalReverting);

            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                foreach (var dto in orders)
                {
                    dto.Order.AmountToWithdraw = Math.Round(dto.AmountToWithdrawNext, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                    dto.Order.AmountWithdrawn = Math.Round(dto.AmountAlreadyWithdrawn, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
                    
                    _orderRepository.Update(dto.Order);
                    scope.Updated<Order>(dto.Order.Id);
                    processedOrders.Add(dto.Order.Id, dto.Order);
                }

                _orderRepository.Save();
                scope.Complete();
            }

            _logger.InfoFormat(
                "Finished actualizing order amount info during withdrawal process. Is reverting: {0}. Orders processed: {1}", 
                isWithdrawalReverting,
                processedOrders.Count);

            return processedOrders;
        }
    }
}