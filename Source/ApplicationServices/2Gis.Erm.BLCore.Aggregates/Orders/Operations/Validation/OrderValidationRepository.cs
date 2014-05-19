using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Validation
{
    public class OrderValidationRepository : IOrderValidationRepository
    {
        private readonly IDictionary<OrderValidationRuleGroup, long> _validationRuleGroupMap;

        private readonly IFinder _finder;
        private readonly IRepository<OrderValidationResult> _orderValidationResultGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderValidationRepository(
            IFinder finder, 
            IRepository<OrderValidationResult> orderValidationResultGenericRepository, 
            IIdentityProvider identityProvider, 
            IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _orderValidationResultGenericRepository = orderValidationResultGenericRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _validationRuleGroupMap = finder.FindAll<Platform.Model.Entities.Erm.OrderValidationRuleGroup>().ToDictionary(x => (OrderValidationRuleGroup)x.Code, x => x.Id);
        }

        public long GetGroupId(OrderValidationRuleGroup orderValidationRuleGroup)
        {
            if (!_validationRuleGroupMap.ContainsKey(orderValidationRuleGroup))
            {
                throw new ArgumentException(string.Format("No mapping provided for group [{0}] with code [{1}]",
                                                          orderValidationRuleGroup, (int)orderValidationRuleGroup),
                                            "orderValidationRuleGroup");
            }

            return _validationRuleGroupMap[orderValidationRuleGroup];
        }

        public int AddValidResult(long orderId, ValidationContext validationContext)
        {
            return AddOrderValidationResult(orderId, validationContext, true);
        }

        public int AddInvalidResult(long orderId, ValidationContext validationContext)
        {
            return AddOrderValidationResult(orderId, validationContext, false);
        }

        public ValidResultsContainer CreateValidResultsContainer(Expression<Func<Order, bool>> filterPredicate, out int orderCount)
        {
            var orderByFirmMaps = _finder.Find(filterPredicate)
                .Select(x => new OrderToFirmMap
                {
                    OrderId = x.Id,
                    FirmId = x.FirmId
                })
                .ToArray();
            orderCount = orderByFirmMaps.Count();
            return new ValidResultsContainer(orderByFirmMaps);
        }

        public void SaveValidResults(ValidResultsContainer validResultsContainer)
        {
            foreach (var validResult in validResultsContainer.ValidResults)
            {
                AddValidResult(validResult.OrderId, validResult.ValidationContext);
            }
        }

        private int AddOrderValidationResult(long orderId, ValidationContext validationContext, bool isValid)
        {
            var orderValidationRuleGroupId = GetGroupId(validationContext.OrderValidationRuleGroup);
            int count;
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderValidationResult>())
            {
                var validationResult = new OrderValidationResult
                {
                    OrderId = orderId,
                    OrderValidationGroupId = orderValidationRuleGroupId,
                    OrderValidationType = (int)validationContext.ValidationType,
                    IsValid = isValid
                };

                _identityProvider.SetFor(validationResult);
                _orderValidationResultGenericRepository.Add(validationResult);
                count = _orderValidationResultGenericRepository.Save();

                scope.Added<OrderValidationResult>(validationResult.Id)
                     .Complete();
            }

            return count;
        }
    }
}