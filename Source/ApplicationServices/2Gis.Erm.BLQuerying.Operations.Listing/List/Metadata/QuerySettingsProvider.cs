using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Metadata
{
    public sealed class QuerySettingsProvider : IQuerySettingsProvider
    {
        private readonly IUIConfigurationService _configurationService;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IAppSettings _appSettings;
        private readonly RelationalMetadata _relationalMetadata;

        public QuerySettingsProvider(
            IUIConfigurationService configurationService,
            IUserContext userContext,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            IAppSettings appSettings)
        {
            _configurationService = configurationService;
            _userContext = userContext;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _appSettings = appSettings;

            // TODO: запилить нормальные метаданные
            _relationalMetadata = new RelationalMetadata(_configurationService, _userContext);
        }

        public QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                SortDirection = searchListModel.Dir,
                ParentEntityName = searchListModel.ParentEntityName,
                ParentEntityId = searchListModel.ParentEntityId,
            };

            // fill from dataListStructure
            var dataListStructure = GetDataListStructure(entityName, searchListModel);
            if (string.IsNullOrEmpty(dataListStructure.MainAttribute))
            {
                throw new ArgumentException(BLResources.MainAttributeForEntityIsNotSpecified);
            }

            querySettings.MainAttribute = dataListStructure.MainAttribute;
            querySettings.UserInputFilter = dataListStructure.GetUserInputFilterPredicate(searchListModel.FilterInput);
            querySettings.Fields = dataListStructure.Fields.Select(x => new QueryField
            {
                Name = x.Name,
                Expression = x.ExpressionPath,
                Filterable = x.Filtered,
            }).ToArray();
            querySettings.SortOrder = dataListStructure.Fields.Where(x => x.Name == searchListModel.Sort).Select(x => x.ExpressionPath).SingleOrDefault();
            querySettings.DefaultFilter = FormatDefaultFilter(dataListStructure.DefaultFilter);

            // fill extended info map
            querySettings.ExtendedInfoMap = ParseExtendedInfo(dataListStructure.ExtendedInfo, searchListModel.ExtendedInfo);

            bool filterToParent;
            if (querySettings.TryGetExtendedProperty("filterToParent", out filterToParent) && querySettings.ParentEntityName != EntityName.None && querySettings.ParentEntityId != null)
            {
                querySettings.FilterPredicate = this._relationalMetadata.GetFilterToParentPredicate(entityName, querySettings.ParentEntityName, querySettings.ParentEntityId.Value);
            }

            return querySettings;
        }

        private static IReadOnlyDictionary<string, string> ParseExtendedInfo(string extendedInfo1, string extendedInfo2)
        {
            if (string.IsNullOrEmpty(extendedInfo1) && string.IsNullOrEmpty(extendedInfo2))
            {
                return new Dictionary<string, string>();
            }

            var extendedInfo = extendedInfo1 + '&' + extendedInfo2;

            var extendedInfoMap = extendedInfo
                .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(x => x.Length == 2 && !string.Equals(x[1], "null", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x[0].ToLowerInvariant(), x => x[1]);

            return extendedInfoMap;
        }

        private string FormatDefaultFilter(string defaultFilter)
        {
            if (defaultFilter == null)
            {
                return string.Empty;
            }

            defaultFilter = defaultFilter.Replace("{systemuserid}", _userContext.Identity.Code.ToString(CultureInfo.InvariantCulture));
            defaultFilter = defaultFilter.Replace("{reserveuserid}", _securityServiceUserIdentifier.GetReserveUserIdentity().Code.ToString(CultureInfo.InvariantCulture));
            defaultFilter = defaultFilter.Replace("{balancedebtborder}", _appSettings.MinDebtAmount.ToString());

            return defaultFilter;
        }

        private DataListStructure GetDataListStructure(EntityName entityName, SearchListModel searchListModel)
        {
            var userCultureInfo = _userContext.Profile.UserLocaleInfo.UserCultureInfo;
            var gridSettings = _configurationService.GetGridSettings(entityName, userCultureInfo);

            // в lookup метод forceGetData не знает о nameLocaleResouceId, поэтому иногда возвращаем dataView по умолчанию
            var dataListStructure = gridSettings.DataViews.FirstOrDefault(x => string.Equals(x.NameLocaleResourceId, searchListModel.NameLocaleResourceId, StringComparison.OrdinalIgnoreCase))
                                    ?? gridSettings.DataViews.First();

            return dataListStructure;
        }

        private sealed class RelationalMetadata
        {
            private static readonly Dictionary<Tuple<EntityName, EntityName>, string> RelationalMap = new Dictionary<Tuple<EntityName, EntityName>, string>
            {
                { Tuple.Create(EntityName.Order, EntityName.OrderPosition), "OrderId={0}" },
                { Tuple.Create(EntityName.LegalPerson, EntityName.LegalPersonProfile), "LegalPersonId={0}" },
                { Tuple.Create(EntityName.OrganizationUnit, EntityName.UserOrganizationUnit), "OrganizationUnitId={0}" },
                { Tuple.Create(EntityName.Advertisement, EntityName.AdvertisementElement), "AdvertisementId={0}" },
            };

            private readonly IUIConfigurationService _uiConfigurationService;
            private readonly IUserContext _userContext;

            public RelationalMetadata(IUIConfigurationService uiConfigurationService, IUserContext userContext)
            {
                _uiConfigurationService = uiConfigurationService;
                _userContext = userContext;
            }

            public string GetFilterToParentPredicate(EntityName entityName, EntityName parentEntityName, long parentEntityId)
            {
                var filterExpression = GetFilterPredicateFromEntitySettings(entityName, parentEntityName, parentEntityId);
                if (filterExpression != null)
                {
                    return filterExpression;
                }

                filterExpression = GetFilterExpressionFromRelationalMap(entityName, parentEntityName, parentEntityId);
                if (filterExpression != null)
                {
                    return filterExpression;
                }

                throw new Exception(string.Format("Relational metadata should be added (Entity={0}, RelatedItem={1})", parentEntityName, entityName));
            }

            private static string GetFilterExpressionFromRelationalMap(EntityName entityName, EntityName parentEntityName, long parentEntityId)
            {
                string filterExpression;
                if (!RelationalMap.TryGetValue(Tuple.Create(parentEntityName, entityName), out filterExpression))
                {
                    return null;
                }

                filterExpression = string.Format(filterExpression, parentEntityId);
                return filterExpression;
            }

            private string GetFilterPredicateFromEntitySettings(EntityName entityName, EntityName parentEntityName, long parentEntityId)
            {
                var userCultureInfo = _userContext.Profile.UserLocaleInfo.UserCultureInfo;
                var cardSettings = _uiConfigurationService.GetCardSettings(parentEntityName, userCultureInfo);

                var filterExpression = cardSettings.CardRelatedItems.SelectMany(x => x.Items).Where(x => x.Name.Contains(entityName.ToString()) || (x.RequestUrl != null && x.RequestUrl.Contains(entityName.ToString()))).Select(x => x.FilterExpression).FirstOrDefault();
                if (filterExpression == null)
                {
                    return null;
                }

                filterExpression = filterExpression.Replace("{parentEntityId}", parentEntityId.ToString(CultureInfo.InvariantCulture));
                return filterExpression;
            }
        }
    }
}