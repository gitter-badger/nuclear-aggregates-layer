using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class DefaultThemeMustBeSpecifiedValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public DefaultThemeMustBeSpecifiedValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            if (!IsCheckMassive)
            {
                throw new InvalidOperationException("Check must be massive");
            }

            var defaultThemes = GetDefaultThemes(request.OrganizationUnitId, request.Period);
            GetValidationMessage(request.OrganizationUnitId, defaultThemes, messages);
        }

        private void GetValidationMessage(long? organizationUnitId, long[] defaultThemes, IList<OrderValidationMessage> messages)
        {
            switch (defaultThemes.Length)
            {
                // Нет тематик по-умолчанию
                case 0:
                    messages.Add(new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        MessageText = string.Format(BLResources.DefaultThemeIsNotSpecified, GetOrganizationUnitDescription(organizationUnitId)),
                    });
                    break;

                // Ровно одна тематика. Ок.
                case 1:
                    break;

                // Более одной тематики по умолчанию. Перестраховка.
                default:
                    messages.Add(new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        MessageText = string.Format(BLResources.MoreThanOneDefaultTheme, GetOrganizationUnitDescription(organizationUnitId)),
                    });
                    break;
            }
        }

        private string GetOrganizationUnitDescription(long? organizationUnitId)
        {
            if (!organizationUnitId.HasValue)
            {
                throw new ArgumentNullException("organizationUnitId");
            }

            var name = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId.Value))
                         .Select(unit => unit.Name)
                         .SingleOrDefault();

            return GenerateDescription(EntityName.OrganizationUnit, name, organizationUnitId.Value);
        }

        private long[] GetDefaultThemes(long? organizationUnitId, TimePeriod period)
        {
            if (!organizationUnitId.HasValue)
            {
                throw new ArgumentNullException("organizationUnitId");
            }

            if (period == null)
            {
                throw new ArgumentNullException("period");
            }

            // Ищем тематику, установленную по умолчанию, которая действует в течении всего указанного периода для указанного отделения 2гис.
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Theme>())
                         .Where(theme => theme.IsDefault &&
                                         theme.BeginDistribution <= period.Start &&
                                         theme.EndDistribution >= period.End)
                         .Select(theme => theme.Id)
                         .ToArray();
        }
    }
}
