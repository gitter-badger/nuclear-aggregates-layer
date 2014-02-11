using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListUserOrganizationUnitService : IListGenericEntityService<UserOrganizationUnit>
    {
        private readonly IQuerySettingsProvider _querySettingsProvider;
        private readonly IFinderBaseProvider _finderBaseProvider;

        public ListUserOrganizationUnitService(IQuerySettingsProvider querySettingsProvider, IFinderBaseProvider finderBaseProvider)
        {
            _querySettingsProvider = querySettingsProvider;
            _finderBaseProvider = finderBaseProvider;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            int count;
            var entityType = typeof(UserOrganizationUnit);
            var entityName = entityType.AsEntityName();

            var finderBase = _finderBaseProvider.GetFinderBase(entityName);
            var query = finderBase.FindAll<UserOrganizationUnit>();

            var querySettings = _querySettingsProvider.GetQuerySettings(entityName, searchListModel);

            var dynamicList = query.Select(x => new
                                       {
                                           x.Id,
                                           x.UserId,
                                           x.OrganizationUnitId,
                                           OrganizationUnitName = x.OrganizationUnitDto.Name,
                                           UserName = x.User.DisplayName,
                                           UserDepartmentName = x.User.Department.Name,
                                           UserRoleName = x.User.UserRoles.Select(y => y.Role.Name)
                                       })
                                   .ApplyQuerySettings(querySettings, out count)
                                   .ToDynamicList(querySettings.Fields);

            return new DynamicListResult
            {
                Data = dynamicList,
                RowCount = count,
                MainAttribute = querySettings.MainAttribute
            };
        }
    }
}