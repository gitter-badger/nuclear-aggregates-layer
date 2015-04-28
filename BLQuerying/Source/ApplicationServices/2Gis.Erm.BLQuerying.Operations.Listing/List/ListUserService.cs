using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserService : ListEntityDtoServiceBase<User, ListUserDto>
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListUserService(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            IUserContext userContext, FilterHelper filterHelper)
        {
            _functionalAccessService = functionalAccessService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<User>();

            // hide service users
            Expression<Func<User, bool>> hideServiceUsersFilter = null;
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ServiceUserAssign, _userContext.Identity.Code))
            {
                hideServiceUsersFilter = x => !x.IsServiceUser;
            }

            var roleFilter = querySettings.CreateForExtendedProperty<User, long>(
                "roleId", roleId => x => x.UserRoles.Any(ur => ur.RoleId == roleId));

            var userOrgUnitFilter = querySettings.CreateForExtendedProperty<User, long>(
                "userIdForOrgUnit",
                userIdForOrgUnit => x => x.UserOrganizationUnits
                                            .Any(y => y.OrganizationUnitDto.UserOrganizationUnits
                                                                        .Any(z => z.UserId == userIdForOrgUnit)));

            var privilegyFilter = querySettings.CreateForExtendedProperty<User, long>(
                "privilege", privilegeId => x => x.UserRoles.Select(y => y.Role).SelectMany(y => y.RolePrivileges).Distinct().Any(y => y.Privilege.Operation == privilegeId));

            // Означает, что список должен содержать только самого пользователя и его подчинённых
            var subordinatesFilter = querySettings.CreateForExtendedProperty<User, long>("subordinatesOf", userId => x => x.Id == userId);
            if (subordinatesFilter != null)
            {
                subordinatesFilter = GetSubordinatesFilter(query, subordinatesFilter);
            }

            return query
                .Filter(_filterHelper
                , roleFilter
                , userOrgUnitFilter
                , privilegyFilter
                , subordinatesFilter
                , hideServiceUsersFilter)
                .Select(x => new ListUserDto
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Account = x.Account,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    ParentId = x.ParentId,
                    ParentName = x.Parent.DisplayName,
                    RoleName = x.UserRoles.Select(role => role.Role.Name).OrderBy(item => item),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    IsServiceUser = x.IsServiceUser,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        private static Expression<Func<User, bool>> GetSubordinatesFilter(IQueryable<User> query, Expression<Func<User, bool>> subordinatesFilter)
        {
            // Формирует фильтр по подчинённым сотрудникам, последовательно получая идентификаторы каждого уровня 
            // и затем фильтруя пользователей по этим идентификаторам
            var rootUserId = query.Where(subordinatesFilter)
                .Select(user => user.Id)
                .Single();

            var result = new List<long>();
            var previousLayer = new[] { rootUserId };
            var protectiveCounter = 0; // защита от вечного цикла в случае наличия циклического подчиненения среди сотрудников
            while (previousLayer.Length > 0 && protectiveCounter < 1000)
            {
                protectiveCounter++;
                result.AddRange(previousLayer);
                previousLayer = query.Where(user => user.ParentId.HasValue && previousLayer.Contains(user.ParentId.Value))
                    .Select(user => user.Id)
                    .ToArray(); 
            }

            return user => result.Contains(user.Id);
        }
    }
}