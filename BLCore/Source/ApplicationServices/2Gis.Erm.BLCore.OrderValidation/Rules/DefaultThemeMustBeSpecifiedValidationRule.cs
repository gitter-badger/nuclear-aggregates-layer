using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class DefaultThemeMustBeSpecifiedValidationRule : OrderValidationRuleBase<MassOverridibleValidationRuleContext>
    {
        private readonly IFinder _finder;

        public DefaultThemeMustBeSpecifiedValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(MassOverridibleValidationRuleContext ruleContext)
        {
            var defaultThemes = GetDefaultThemes(ruleContext.ValidationParams.OrganizationUnitId, ruleContext.ValidationParams.Period);
            return GetValidationMessage(ruleContext.ValidationParams.OrganizationUnitId, defaultThemes);
        }

        private IEnumerable<OrderValidationMessage> GetValidationMessage(long organizationUnitId, IReadOnlyCollection<long> defaultThemes)
        {
            switch (defaultThemes.Count)
            {
                // Нет тематик по-умолчанию
                case 0:
                    return new[]
                               {
                                   new OrderValidationMessage
                                       {
                                           Type = MessageType.Error,
                                           MessageText = string.Format(BLResources.DefaultThemeIsNotSpecified, GetOrganizationUnitDescription(organizationUnitId)),
                                       }
                               };

                // Ровно одна тематика. Ок.
                case 1:
                    return Enumerable.Empty<OrderValidationMessage>();

                // Более одной тематики по умолчанию. Перестраховка.
                default:
                    return new[]
                               {
                                   new OrderValidationMessage
                                       {
                                           Type = MessageType.Error,
                                           MessageText = string.Format(BLResources.MoreThanOneDefaultTheme, GetOrganizationUnitDescription(organizationUnitId))
                                       }
                               };
            }
        }

        private string GetOrganizationUnitDescription(long organizationUnitId)
        {
            var name = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                              .Map(q => q.Select(unit => unit.Name))
                              .One();

            return GenerateDescription(true, EntityType.Instance.OrganizationUnit(), name, organizationUnitId);
        }

        // TODO {all, 02.10.2014}: разобраться почему не используется organizationUnitId 
        private IReadOnlyCollection<long> GetDefaultThemes(long organizationUnitId, TimePeriod period)
        {
            // Ищем тематику, установленную по умолчанию, которая действует в течении всего указанного периода для указанного отделения 2гис.
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Theme>() &&
                                new FindSpecification<Theme>(theme => theme.IsDefault &&
                                                                      theme.BeginDistribution <= period.Start &&
                                                                      theme.EndDistribution >= period.End))
                          .Map(q => q.Select(theme => theme.Id))
                          .Many();
        }
    }
}
