using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class SalesModelRestrictionsOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private const int CategoryLevelToCheck = 3;
        private readonly IFinder _finder;

        public SalesModelRestrictionsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var badAdvertisemements =
                _finder.Find(ruleContext.OrdersFilterPredicate)
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
                                                                                                           EntityName.OrderPosition,
                                                                                                           x.OrderPositionName,
                                                                                                           x.OrderPositionId),
                                                                                       GenerateDescription(ruleContext.ValidationParams.IsMassValidation,
                                                                                                           EntityName.Category,
                                                                                                           x.CategoryName,
                                                                                                           x.CategoryId),
                                                                                       x.ProjectName)
                                                       });
        }
    }
}