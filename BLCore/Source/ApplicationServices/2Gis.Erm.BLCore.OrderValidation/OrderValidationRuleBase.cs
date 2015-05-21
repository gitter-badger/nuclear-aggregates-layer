using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public abstract class OrderValidationRuleBase<TValidationRuleContext> : IOrderValidationRule
        where TValidationRuleContext : class, IValidationRuleContext
    {
        private readonly IReadOnlyDictionary<Type, RuleContextFactory> _ruleContextFactories;

        protected OrderValidationRuleBase()
        {
            _ruleContextFactories = new Dictionary<Type, RuleContextFactory>
                                        {
                                            { typeof(OrdinaryValidationRuleContext), OrdinaryContextFactory },
                                            { typeof(HybridParamsValidationRuleContext), HybridContextFactory },
                                            { typeof(MassOverridibleValidationRuleContext), MassContextFactory },
                                            { typeof(BrowsableResultsValidationRuleContext), BrowsableContextFactory },
                                            { typeof(SingleValidationRuleContext), SingleContextFactory },
                                        };
        }

        protected delegate IValidationRuleContext RuleContextFactory(ValidationParams validationParams,
                                                                     OrderValidationPredicate combinedPredicate,
                                                                     IValidationResultsBrowser validationResultsBrowser);

        IEnumerable<OrderValidationMessage> IOrderValidationRule.Validate(
            ValidationParams validationParams, 
            OrderValidationPredicate combinedPredicate,
            IValidationResultsBrowser validationResultsBrowser)
        {
            var ruleContextType = typeof(TValidationRuleContext);
            RuleContextFactory ruleContextFactory;
            if (!_ruleContextFactories.TryGetValue(ruleContextType, out ruleContextFactory))
            {
                throw new NotSupportedException("Specified rule context type " + ruleContextType + "is not supported");
            }

            return Validate((TValidationRuleContext)ruleContextFactory(validationParams, combinedPredicate, validationResultsBrowser));
        }

        protected abstract IEnumerable<OrderValidationMessage> Validate(TValidationRuleContext ruleContext);

        protected string GenerateDescription(bool isMassValidation, IEntityType entityName, string description, long entityId)
        {
            return isMassValidation ? description : string.Format("<{0}:{1}:{2}>", entityName.Description, description, entityId);
        }

        protected Expression<Func<Order, bool>> GetFilterPredicateToGetLinkedOrders(IQuery query, long orderId, out long organizationUnitId, out long? firmId)
        {
            if (orderId == 0)
            {
                throw new ArgumentNullException("orderId");
            }

            var orderInfo = query.For(Specs.Find.ById<Order>(orderId))
                                  .Select(item => new 
                                      {
                                            item.BeginReleaseNumber,
                                            item.DestOrganizationUnitId,
                                            EndReleaseNumber = item.EndReleaseNumberFact,
                                            item.FirmId
                                      })
                                  .Single();

            organizationUnitId = orderInfo.DestOrganizationUnitId;
            firmId = orderInfo.FirmId;

            return order => order.IsActive && !order.IsDeleted &&
                            order.DestOrganizationUnitId == orderInfo.DestOrganizationUnitId &&
                            (order.Id == orderId ||
                             order.WorkflowStepId == OrderState.OnApproval ||
                             order.WorkflowStepId == OrderState.Approved ||
                             order.WorkflowStepId == OrderState.OnTermination) &&
                            ((order.BeginReleaseNumber >= orderInfo.BeginReleaseNumber && order.BeginReleaseNumber <= orderInfo.EndReleaseNumber) ||
                             (order.EndReleaseNumberFact >= orderInfo.BeginReleaseNumber && order.EndReleaseNumberFact <= orderInfo.EndReleaseNumber) ||
                             (orderInfo.BeginReleaseNumber >= order.BeginReleaseNumber && orderInfo.BeginReleaseNumber <= order.EndReleaseNumberFact) ||
                             (orderInfo.EndReleaseNumber >= order.BeginReleaseNumber && orderInfo.EndReleaseNumber <= order.EndReleaseNumberFact));
        }

        private static OrdinaryValidationRuleContext OrdinaryContextFactory(
            ValidationParams validationParams,
            OrderValidationPredicate combinedPredicate,
            IValidationResultsBrowser validationResultsBrowser)
        {
            return new OrdinaryValidationRuleContext(validationParams is MassOrdersValidationParams, combinedPredicate.GetCombinedPredicate());
        }

        private static HybridParamsValidationRuleContext HybridContextFactory(
            ValidationParams validationParams,
            OrderValidationPredicate combinedPredicate,
            IValidationResultsBrowser validationResultsBrowser)
        {
            return new HybridParamsValidationRuleContext(new HybridValidationParams(validationParams), combinedPredicate.GetCombinedPredicate());
        }

        private static MassOverridibleValidationRuleContext MassContextFactory(
            ValidationParams validationParams,
            OrderValidationPredicate combinedPredicate,
            IValidationResultsBrowser validationResultsBrowser)
        {
            return new MassOverridibleValidationRuleContext((MassOrdersValidationParams)validationParams, combinedPredicate);
        }

        private static BrowsableResultsValidationRuleContext BrowsableContextFactory(
            ValidationParams validationParams,
            OrderValidationPredicate combinedPredicate,
            IValidationResultsBrowser validationResultsBrowser)
        {
            return new BrowsableResultsValidationRuleContext(new HybridValidationParams(validationParams), combinedPredicate.GetCombinedPredicate(), validationResultsBrowser);
        }

        private static SingleValidationRuleContext SingleContextFactory(
            ValidationParams validationParams,
            OrderValidationPredicate combinedPredicate,
            IValidationResultsBrowser validationResultsBrowser)
        {
            return new SingleValidationRuleContext((SingleOrderValidationParams)validationParams);
        }
    }
}
