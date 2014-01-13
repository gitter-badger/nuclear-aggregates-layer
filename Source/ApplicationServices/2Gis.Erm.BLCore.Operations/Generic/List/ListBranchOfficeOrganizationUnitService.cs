using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListBranchOfficeOrganizationUnitService : IListGenericEntityService<BranchOfficeOrganizationUnit>
    {
        private readonly IQuerySettingsProvider _querySettingsProvider;
        private readonly IFinderBaseProvider _finderBaseProvider;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public ListBranchOfficeOrganizationUnitService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _querySettingsProvider = querySettingsProvider;
            _finderBaseProvider = finderBaseProvider;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            int count;
            var entityType = typeof(BranchOfficeOrganizationUnit);
            var entityName = entityType.AsEntityName();

            var finderBase = _finderBaseProvider.GetFinderBase(entityName);
            var query = finderBase.FindAll<BranchOfficeOrganizationUnit>();

            var querySettings = _querySettingsProvider.GetQuerySettings(entityName, searchListModel);
            var data = GetListData(query, querySettings, new ListFilterManager(searchListModel), out count);

            return new DynamicListResult
            {
                Data = data,
                RowCount = count,
                MainAttribute = querySettings.MainAttribute
            };
        }

        private IEnumerable<DynamicListRow> GetListData(IQueryable<BranchOfficeOrganizationUnit> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var filter = filterManager.CreateForExtendedProperty<BranchOfficeOrganizationUnit, long, bool>(
                "userId", 
                "restrictByFP",
                ExtendedPropertyUnionType.Or,
                (userId, restrictByPrivelege) =>
                {
                    if (restrictByPrivelege)
                    {
                        var hasPrivilege = _functionalAccessService
                            .HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection, _userContext.Identity.Code);
                        if (!hasPrivilege)
                        {
                            return null;
                        }
                        else
                        {
                            Expression<Func<BranchOfficeOrganizationUnit, bool>> filterExpression =
                                x => x.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);

                            return filterExpression;
                        }
                    }
                    return null;
                });

            if (filter == null)
            {
                filter = filterManager.CreateForExtendedProperty<BranchOfficeOrganizationUnit, long, bool>(
                    "sourceOrganizationUnitId", 
                    "restrictByFP",
                    ExtendedPropertyUnionType.Or,
                    (sourceOrganizationUnitId, restrictByPrivelege) =>
                        {
                            Expression<Func<BranchOfficeOrganizationUnit, bool>> filterExpression =
                                x => x.OrganizationUnitId == sourceOrganizationUnitId;

                            if (restrictByPrivelege)
                            {
                                var hasPrivilege = _functionalAccessService
                                    .HasFunctionalPrivilegeGranted(
                                        FunctionalPrivilegeName.OrderBranchOfficeOrganizationUnitSelection,
                                        _userContext.Identity.Code);
                                if (!hasPrivilege)
                                {
                                    return filterExpression;
                                }
                            }
                            else
                            {
                                return filterExpression;
                            }

                            return null;
                        });
            }

            var dynamicList = query
                .ApplyFilter(filter)
                .ApplyQuerySettings(querySettings, out count)
                .ToDynamicList(querySettings.Fields);

            return dynamicList;
        }
    }
}