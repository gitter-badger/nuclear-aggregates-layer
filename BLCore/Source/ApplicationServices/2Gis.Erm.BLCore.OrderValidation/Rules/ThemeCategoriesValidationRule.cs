using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ThemeCategoriesValidationRule : OrderValidationRuleBase<MassOverridibleValidationRuleContext>
    {
        private readonly IQuery _query;

        public ThemeCategoriesValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(MassOverridibleValidationRuleContext ruleContext)
        {
            // Все тематики, использованные в заказах, которые удовлетворяют фильтру, переданному нам свыше
            var themes = _query.For<Order>()
                               .Where(ruleContext.CombinedPredicate.GetCombinedPredicate())
                               .SelectMany(order => order.OrderPositions)
                               .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                               .SelectMany(position => position.OrderPositionAdvertisements)
                               .Where(advertisement => advertisement.ThemeId.HasValue)
                               .Select(advertisement => advertisement.ThemeId.Value)
                               .Distinct()
                               .ToArray();

            // Линки на невалидные рубрики для тематик, использованных в заказах
            var invalidCategories = _query.For(new FindSpecification<ThemeCategory>(link => !link.IsDeleted && themes.Contains(link.ThemeId)))
                          .Where(link => !link.Category.IsActive || link.Category.IsDeleted)
                          .Select(link => new { link.Theme, link.Category })
                          .ToArray();

            return invalidCategories.Select(x => new OrderValidationMessage
                                                     {
                                                         Type = MessageType.Error,
                                                         MessageText =
                                                             string.Format(BLResources.ThemeUsesInactiveCategory,
                                                                           GenerateDescription(true, EntityType.Instance.Theme(), x.Theme.Name, x.Theme.Id),
                                                                           GenerateDescription(true, EntityType.Instance.Category(), x.Category.Name, x.Category.Id))
                                                     });
        }
    }
}
