using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
            var destOrganizationUnitId = ruleContext.ValidationParams.IsMassValidation
                                             ? ruleContext.ValidationParams.Mass.OrganizationUnitId
                                             : _finder.Find(Specs.Find.ById<Order>(ruleContext.ValidationParams.Single.OrderId)).Select(x => x.DestOrganizationUnitId).Single();

            var projectInfo = _finder.Find(Specs.Find.ById<OrganizationUnit>(destOrganizationUnitId))
                                     .Select(x => x.Projects.FirstOrDefault())
                                     .Select(x => new
                                                      {
                                                          Id = x.Id,
                                                          Name = x.DisplayName
                                                      })
                                     .Single();

            var badAdvertisemements =
                _finder.Find(ruleContext.OrdersFilterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive
                                               && !orderPosition.IsDeleted)
                       .SelectMany(orderPosition =>
                                   orderPosition.OrderPositionAdvertisements.Where(opa =>
                                                                                   opa.CategoryId.HasValue
                                                                                   && opa.Category.Level == CategoryLevelToCheck
                                                                                   && !opa.Category.SalesModelRestrictions.Any(sr =>
                                                                                                                               sr.SalesModel == opa.Position.SalesModel &&
                                                                                                                               sr.ProjectId == projectInfo.Id))
                                                .Select(advertisement => new
                                                                             {
                                                                                 OrderPositionId = orderPosition.Id,
                                                                                 OrderPositionName = advertisement.Position.Name,
                                                                                 OrderId = orderPosition.Order.Id,
                                                                                 OrderNumber = orderPosition.Order.Number,
                                                                                 CategoryId = advertisement.CategoryId.Value,
                                                                                 CategoryName = advertisement.Category.Name,
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
                                                                                       projectInfo.Name)
                                                       });
        }
    }
}