using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class DefaultFilter
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public DefaultFilter(IUserContext userContext, ISecurityServiceUserIdentifier securityServiceUserIdentifier, IDebtProcessingSettings debtProcessingSettings)
        {
            _userContext = userContext;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _debtProcessingSettings = debtProcessingSettings;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> queryable, QuerySettings querySettings)
        {
            if (string.IsNullOrEmpty(querySettings.DefaultFilter))
            {
                return queryable;
            }

            var formattedDefaultFilter = FormatDefaultFilter(querySettings.DefaultFilter);
            queryable = System.Linq.Dynamic.DynamicQueryable.Where(queryable, formattedDefaultFilter);

            return queryable;
        }

        private string FormatDefaultFilter(string defaultFilter)
        {
            if (defaultFilter == null)
            {
                return string.Empty;
            }

            defaultFilter = defaultFilter.Replace("{systemuserid}", _userContext.Identity.Code.ToString(CultureInfo.InvariantCulture));
            defaultFilter = defaultFilter.Replace("{reserveuserid}", _securityServiceUserIdentifier.GetReserveUserIdentity().Code.ToString(CultureInfo.InvariantCulture));
            defaultFilter = defaultFilter.Replace("{balancedebtborder}", _debtProcessingSettings.MinDebtAmount.ToString());

            return defaultFilter;
        }
    }
}