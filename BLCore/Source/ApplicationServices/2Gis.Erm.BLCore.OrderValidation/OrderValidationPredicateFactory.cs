using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    // TODO {all, 29.09.2014}: возможно, данный функционал лучше будет вынести, например, в readmodel или просто util метод
    public sealed class OrderValidationPredicateFactory : IOrderValidationPredicateFactory
    {
        private readonly IFinder _finder;

        public OrderValidationPredicateFactory(IFinder finder)
        {
            _finder = finder;
        }

        public OrderValidationPredicate CreatePredicate(ValidationParams validationParams)
        {
            var singleOrderValidationParams = validationParams as SingleOrderValidationParams;
            if (singleOrderValidationParams != null)
            {
                return new OrderValidationPredicate(x => x.Id == singleOrderValidationParams.OrderId, null, null);
            }

            var massOrdersValidationParams = validationParams as MassOrdersValidationParams;
            if (massOrdersValidationParams != null)
            {
                Expression<Func<Order, bool>> orgUnitPart =
                    x => x.DestOrganizationUnitId == massOrdersValidationParams.OrganizationUnitId
                         || x.SourceOrganizationUnitId == massOrdersValidationParams.OrganizationUnitId;

                OrderValidationPredicate validationPredicate;
                if (massOrdersValidationParams.OwnerId.HasValue)
                {
                    var userDescendantsQuery = _finder.For<UsersDescendant>();

                    // необходимо уточнить условия фильтрации для заказов уходящих в выпуск
                    validationPredicate = new OrderValidationPredicate(
                        x => x.IsActive && !x.IsDeleted &&
                             (x.WorkflowStepId == OrderState.Approved ||
                              (x.WorkflowStepId == OrderState.OnTermination && x.EndDistributionDateFact >= massOrdersValidationParams.Period.End)) &&
                             x.BeginDistributionDate < massOrdersValidationParams.Period.End && x.EndDistributionDateFact > massOrdersValidationParams.Period.Start &&
                             (x.OwnerCode == massOrdersValidationParams.OwnerId || (massOrdersValidationParams.IncludeOwnerDescendants &&
                                                                                    userDescendantsQuery.Any(
                                                                                                             ud =>
                                                                                                             ud.AncestorId == massOrdersValidationParams.OwnerId &&
                                                                                                             ud.DescendantId == x.OwnerCode))),
                        orgUnitPart,
                        null);
                }
                else
                {
                    validationPredicate = new OrderValidationPredicate(
                        x => x.IsActive && !x.IsDeleted &&
                             (x.WorkflowStepId == OrderState.Approved ||
                              (x.WorkflowStepId == OrderState.OnTermination && x.EndDistributionDateFact >= massOrdersValidationParams.Period.End)) &&
                             x.BeginDistributionDate < massOrdersValidationParams.Period.End && x.EndDistributionDateFact > massOrdersValidationParams.Period.Start,
                        orgUnitPart,
                        null);
                }

                return validationPredicate;
            }

            throw new NotSupportedException("Specified type of valdation params is not supported: " + validationParams.GetType());
        }
    }
}