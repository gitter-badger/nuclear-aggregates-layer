using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить отделение организации выбранных сущностей "Рубрика фирмы" на соотнесение с отделением организации города назначения заказа
    /// </summary>
    public sealed class CategoriesLinkedToDestOrgUnitOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public CategoriesLinkedToDestOrgUnitOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var badOrderPositions = _query.For<Order>()
                .Where(ruleContext.OrdersFilterPredicate)
                .SelectMany(order => order.OrderPositions)
                .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                .Select(orderPosition =>
                    new
                    {
                        orderPosition.OrderId,
                        OrderNumber = orderPosition.Order.Number,
                        OrderPositionId = orderPosition.Id,
                        BadCategories = orderPosition.OrderPositionAdvertisements.Where(item => item.Category != null)
                                                                                .Select(item => new { Category = item.Category, PositionName = item.Position.Name })
                                                                                .Where(item => !item.Category.CategoryOrganizationUnits.Any(link => link.IsActive && !link.IsDeleted && (link.OrganizationUnit == orderPosition.Order.DestOrganizationUnit))
                                                                                                && !item.Category.ChildCategories.Any(child => child.IsActive
                                                                                                                                    && !child.IsDeleted
                                                                                                                                    && child.ChildCategories.Any(grandChild => grandChild.IsActive
                                                                                                                                                                                && !grandChild.IsDeleted
                                                                                                                                                                                && grandChild.CategoryOrganizationUnits.Any(link => link.IsActive
                                                                                                                                                                                                                                    && !link.IsDeleted
                                                                                                                                                                                                                                    && link.OrganizationUnitId == orderPosition.Order.DestOrganizationUnitId))))
                                                                                .Select(item => new { item.Category.Id, item.Category.Name, item.PositionName }).Distinct()
                    })
                .Where(orderPosition => orderPosition.BadCategories.Any())
                .ToArray();
            
            var results = new List<OrderValidationMessage>();
            foreach (var orderPosition in badOrderPositions)
            {
                var stringBuilder = new StringBuilder();

                var was = false;
                foreach (var item in orderPosition.BadCategories.OrderBy(item => item.Name))
                {
                    var orderPositionDescription = GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.OrderPosition(), item.PositionName, orderPosition.OrderPositionId);
                    stringBuilder.AppendFormat(BLResources.OrderCheckOrderPositionContainsCategoriesFromWrongOrganizationUnit, orderPositionDescription);
                    if (was)
                    {
                        stringBuilder.Append(BLResources.ListSeparator);
                    }

                    was = true;

                    stringBuilder.Append(GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Category(), item.Name, item.Id));
                    results.Add(new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = orderPosition.OrderId,
                            OrderNumber = orderPosition.OrderNumber,
                            MessageText = stringBuilder.ToString()
                        });
                }
            }

            return results;
        }
    }
}
