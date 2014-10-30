using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class OrderValidationPredicateFactory : IOrderValidationPredicateFactory
    {
        private readonly IFinder _finder;

        public OrderValidationPredicateFactory(IFinder finder)
        {
            _finder = finder;
        }

        public OrderValidationPredicate CreatePredicate(long orderId, out OrderState currentOrderState, out TimePeriod period)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(orderId))
                                   .Select(order => new
                                       {
                                           OrderState = order.WorkflowStepId,
                                           order.BeginDistributionDate
                                       })
                                   .Single();

            currentOrderState = orderInfo.OrderState;
            period = new TimePeriod(orderInfo.BeginDistributionDate, orderInfo.BeginDistributionDate.AddMonths(1).AddSeconds(-1));
            
            return new OrderValidationPredicate(x => x.Id == orderId, null, null);
        }

        public OrderValidationPredicate CreatePredicate(long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants)
        {
            Expression<Func<Order, bool>> orgUnitPart = x => x.DestOrganizationUnitId == organizationUnitId || x.SourceOrganizationUnitId == organizationUnitId;

            OrderValidationPredicate validationPredicate;
            if (ownerCode.HasValue)
            {
                var userDescendantsQuery = _finder.FindAll<UsersDescendant>();

                // необходимо уточнить условия фильтрации для заказов уходящих в выпуск
                validationPredicate = new OrderValidationPredicate(
                    x => x.IsActive && !x.IsDeleted &&
                         (x.WorkflowStepId == OrderState.Approved || (x.WorkflowStepId == OrderState.OnTermination && x.EndDistributionDateFact >= period.End)) &&
                         x.BeginDistributionDate < period.End && x.EndDistributionDateFact > period.Start &&
                         (x.OwnerCode == ownerCode || (includeOwnerDescendants &&
                                                       userDescendantsQuery.Any(ud => ud.AncestorId == ownerCode && ud.DescendantId == x.OwnerCode))),
                    orgUnitPart,
                    null);
            }
            else
            {
                validationPredicate = new OrderValidationPredicate(
                    x => x.IsActive && !x.IsDeleted &&
                         (x.WorkflowStepId == OrderState.Approved || (x.WorkflowStepId == OrderState.OnTermination && x.EndDistributionDateFact >= period.End)) &&
                         x.BeginDistributionDate < period.End && x.EndDistributionDateFact > period.Start,
                    orgUnitPart, 
                    null);
            }

            return validationPredicate;
        }
    }
}