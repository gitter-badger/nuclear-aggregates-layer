using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation
{
    public interface IOrderValidationRepository : IAggregateRootRepository<OrderValidationResult>
    {
        int AddValidResult(long orderId, ValidationContext validationContext);
        int AddInvalidResult(long orderId, ValidationContext validationContext);
        ValidResultsContainer CreateValidResultsContainer(Expression<Func<Order, bool>> filterPredicate, out int orderCount);
        void SaveValidResults(ValidResultsContainer validResultsContainer);
    }
}