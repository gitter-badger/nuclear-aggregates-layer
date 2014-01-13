using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders
{
    public sealed class ValidationContext
    {
        public OrderValidationRuleGroup OrderValidationRuleGroup { get; private set; }
        public ValidationType ValidationType { get; private set; }

        public ValidationContext(OrderValidationRuleGroup orderValidationRuleGroup, ValidationType validationType)
        {
            OrderValidationRuleGroup = orderValidationRuleGroup;
            ValidationType = validationType;
        }
    }

    internal sealed class ValidResult
    {
        public long OrderId { get; set; }
        public ValidationContext ValidationContext { get; set; }
    }

    internal sealed class OrderToFirmMap
    {
        public long OrderId { get; set; }
        public long? FirmId { get; set; }
    }

    public sealed class ValidResultsContainer
    {
        private readonly IEnumerable<long> _orderIds;
        private readonly IDictionary<long, IEnumerable<long>> _ordersByFirm;
        private readonly IDictionary<long, long> _firmByOrder;
        private readonly List<ValidResult> _validResults;

        internal IEnumerable<ValidResult> ValidResults
        {
            get { return _validResults.AsReadOnly(); }
        }

        internal ValidResultsContainer(OrderToFirmMap[] orderToFirmMaps)
        {
            _validResults = new List<ValidResult>();

            _orderIds = orderToFirmMaps.Select(x => x.OrderId).ToArray();
            // ReSharper disable PossibleInvalidOperationException
            _ordersByFirm = orderToFirmMaps
                .Where(x => x.FirmId.HasValue)
                .GroupBy(x => x.FirmId.Value)
                .ToDictionary(x => x.Key, x => x.Select(y => y.OrderId));
            _firmByOrder = orderToFirmMaps.Where(x => x.FirmId.HasValue).ToDictionary(x => x.OrderId, x => x.FirmId.Value);
            // ReSharper restore PossibleInvalidOperationException
        }

        public void AppendValidResults(IEnumerable<long> invalidOrderIds, ValidationContext validationContext)
        {
            var extentedInvalidOrderIds = (from invalidOrderId in invalidOrderIds
                                           join firmByOrder in _firmByOrder on invalidOrderId equals firmByOrder.Key
                                           join ordersByFirmItem in _ordersByFirm on firmByOrder.Value equals ordersByFirmItem.Key
                                           from orderId in ordersByFirmItem.Value
                                           select orderId)
                .Distinct()
                .ToArray();

            var validOrderIds = _orderIds.Except(extentedInvalidOrderIds).ToArray();
            foreach (var validOrderId in validOrderIds)
            {
                _validResults.Add(new ValidResult {OrderId = validOrderId, ValidationContext = validationContext});
            }
        }
    }

    public interface IOrderValidationRepository : IAggregateRootRepository<OrderValidationResult>
    {
        long GetGroupId(OrderValidationRuleGroup orderValidationRuleGroup);
        int AddValidResult(long orderId, ValidationContext validationContext);
        int AddInvalidResult(long orderId, ValidationContext validationContext);
        ValidResultsContainer CreateValidResultsContainer(Expression<Func<Order, bool>> filterPredicate, out int orderCount);
        void SaveValidResults(ValidResultsContainer validResultsContainer);
    }
    
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