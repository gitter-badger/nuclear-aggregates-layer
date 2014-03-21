using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ThemeCategoriesValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public ThemeCategoriesValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            if (!IsCheckMassive)
            {
                throw new InvalidOperationException("Check must be massive");
            }

            // Все тематики, использованные в заказах, которые удовлетворяют фильтру, переданному нам свыше
            var themes = _finder.Find(filterPredicate)
                               .SelectMany(order => order.OrderPositions)
                               .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                               .SelectMany(position => position.OrderPositionAdvertisements)
                               .Where(advertisement => advertisement.ThemeId.HasValue)
                               .Select(advertisement => advertisement.ThemeId.Value)
                               .Distinct()
                               .ToArray();

            // Линки на невалидные рубрики для тематик, использованных в заказах
            var invalidCategories = _finder.Find<ThemeCategory>(link => !link.IsDeleted && themes.Contains(link.ThemeId))
                          .Where(link => !link.Category.IsActive || link.Category.IsDeleted)
                          .Select(link => new { link.Theme, link.Category })
                          .ToArray();

            foreach (var invalid in invalidCategories)
            {
                var themeLabel = GenerateDescription(EntityName.Theme, invalid.Theme.Name, invalid.Theme.Id);
                var categoryLabel = GenerateDescription(EntityName.Category, invalid.Category.Name, invalid.Category.Id);
                var message = new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        MessageText = string.Format(BLResources.ThemeUsesInactiveCategory, themeLabel, categoryLabel)
                    };
                messages.Add(message);
            }
        }
    }
}
