using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Metadata
{
    public sealed class QuerySettingsProvider : IQuerySettingsProvider
    {
        private readonly IUIConfigurationService _configurationService;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IAppSettings _appSettings;

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
        }

        public QuerySettings GetQuerySettings(EntityName entityName, SearchListModel searchListModel)
        {
            var gridSettings = _configurationService.GetGridSettings(entityName, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
            return CreateQuerySettings(gridSettings, searchListModel);
        }

        private QuerySettings CreateQuerySettings(EntityDataListsContainer entityViewSet, SearchListModel settings)
        {
            // из-за какого-то хака в lookup field не передаётся NameLocaleResourceId в GetDataList, поэтому приходится брать первый попавшийся
            var dataView = entityViewSet.DataViews.FirstOrDefault(x => string.Equals(x.NameLocaleResourceId,
                                                                                     settings.NameLocaleResourceId,
                                                                                     StringComparison.OrdinalIgnoreCase))
                           ?? entityViewSet.DataViews.First();
            
            if (!string.IsNullOrEmpty(dataView.ExtendedInfo))
            {
                if (string.IsNullOrEmpty(settings.ExtendedInfo))
                {
                    settings.ExtendedInfo = dataView.ExtendedInfo;
                }
                else 
                {
                    settings.ExtendedInfo += string.Format(";{0}", dataView.ExtendedInfo);
                }
            }

            if (string.IsNullOrEmpty(dataView.MainAttribute))
            {
                throw new ArgumentException(BLResources.MainAttributeForEntityIsNotSpecified);
            }

            var filter = MakeFilter(dataView);

            return new QuerySettings
            {
                SkipCount = settings.Start,
                TakeCount = settings.Limit,
                SortOrder = dataView.Fields.Where(x => x.Name == settings.Sort).Select(x => x.ExpressionPath).SingleOrDefault(),
                SortDirection = settings.Dir,
                FilterPredicate = settings.WhereExp,
                Fields = dataView.Fields.Select(x => new QueryField { Name = x.Name, Expression = x.ExpressionPath, Filterable = x.Filtered }).ToArray(),
                DefaultFilter = filter,
                UserInputFilter = dataView.GetUserInputFilterPredicate(settings.FilterInput),
                MainAttribute = dataView.MainAttribute,
            };
        }

        private string MakeFilter(DataListStructure dataView)
        {
            if (dataView.DefaultFilter == null)
            {
                return string.Empty;
            }

            var defaultFilter = dataView.DefaultFilter.Replace("{systemuserid}", _userContext.Identity.Code.ToString(CultureInfo.InvariantCulture));
            defaultFilter = defaultFilter.Replace("{reserveuserid}",
                                                  _securityServiceUserIdentifier.GetReserveUserIdentity().Code.ToString(CultureInfo.InvariantCulture));

            defaultFilter = defaultFilter.Replace("{balancedebtborder}", _appSettings.MinDebtAmount.ToString());
            
            return defaultFilter;
        }
    }
}