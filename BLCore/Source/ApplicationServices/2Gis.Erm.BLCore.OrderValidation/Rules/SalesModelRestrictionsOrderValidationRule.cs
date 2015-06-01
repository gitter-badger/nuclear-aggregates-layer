using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class SalesModelRestrictionsOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private const int CategoryLevelToCheck = 3;
        private readonly IQuery _query;

        public SalesModelRestrictionsOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var badAdvertisemements =
                _query.For<Order>()
                       .Where(ruleContext.OrdersFilterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive
                                               && !orderPosition.IsDeleted)
                       .SelectMany(orderPosition =>
                                   orderPosition.OrderPositionAdvertisements.Where(opa =>
                                                                                   opa.CategoryId.HasValue
                                                                                   && opa.Category.Level == CategoryLevelToCheck
                                                                                   && opa.Position.PositionsGroup != PositionsGroup.Media
                                                                                   && !opa.Category.SalesModelRestrictions.Any(sr =>
                                                                                                                               sr.SalesModel == opa.Position.SalesModel &&
                                                                                                                               sr.ProjectId ==
                                                                                                                               orderPosition.Order.DestOrganizationUnit.Projects
                                                                                                                                            .FirstOrDefault().Id))
                                                .Select(advertisement => new
                                                                             {
                                                                                 OrderPositionId = orderPosition.Id,
                                                                                 OrderPositionName = advertisement.Position.Name,
                                                                                 OrderId = orderPosition.Order.Id,
                                                                                 OrderNumber = orderPosition.Order.Number,
                                                                                 CategoryId = advertisement.CategoryId.Value,
                                                                                 CategoryName = advertisement.Category.Name,
                                                                                 ProjectName = orderPosition.Order.DestOrganizationUnit.Projects
                                                                                                            .FirstOrDefault().DisplayName
                                                                             }))
                       .ToArray();

            return badAdvertisemements.Select(x => new OrderValidationMessage
                                                       {
                                                           Type = MessageType.Error,
                                                           OrderId = x.OrderId,
                                                           OrderNumber = x.OrderNumber,
                                                           MessageText = string.Format(BLResources.CategoryIsRestrictedForSpecifiedSalesModelError,
                                                                                       GenerateDescription(ruleContext.ValidationParams.IsMassValidation,
                                                                                                           EntityType.Instance.OrderPosition(),
                                                                                                           x.OrderPositionName,
                                                                                                           x.OrderPositionId),
                                                                                       GenerateDescription(ruleContext.ValidationParams.IsMassValidation,
                                                                                                           EntityType.Instance.Category(),
                                                                                                           x.CategoryName,
                                                                                                           x.CategoryId),
                                                                                       x.ProjectName)
                                                       });
        }
    }
}