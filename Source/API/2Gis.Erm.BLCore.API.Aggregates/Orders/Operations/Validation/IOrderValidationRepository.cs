using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation
{
    public interface IOrderValidationRepository : IAggregateRootRepository<OrderValidationResult>
    {
        long GetGroupId(OrderValidationRuleGroup orderValidationRuleGroup);
        int AddValidResult(long orderId, ValidationContext validationContext);
        int AddInvalidResult(long orderId, ValidationContext validationContext);
        ValidResultsContainer CreateValidResultsContainer(Expression<Func<Order, bool>> filterPredicate, out int orderCount);
        void SaveValidResults(ValidResultsContainer validResultsContainer);
    }
}