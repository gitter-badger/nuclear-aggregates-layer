using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListUserService : ListEntityDtoServiceBase<User, ListUserDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public ListUserService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IFinder finder,
            IUserContext userContext)
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        protected override IEnumerable<ListUserDto> GetListData(IQueryable<User> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            var hideReserveUserFilter = filterManager.CreateForExtendedProperty<User, bool>(
                "hideReserveUser",
                hideReserveUser =>
                    {
                        var reserveUserId = _userIdentifierService.GetReserveUserIdentity().Code;
                        return x => x.Id != reserveUserId;
                    });

            // hide service users
            Expression<Func<User, bool>> hideServiceUsersFilter = null;
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ServiceUserAssign, _userContext.Identity.Code))
            {
                hideServiceUsersFilter = x => !x.IsServiceUser;
            }

            var roleFilter = filterManager.CreateForExtendedProperty<User, long>(
                "roleId", roleId => x => x.UserRoles.Any(ur => ur.RoleId == roleId));

            var userOrgUnitFilter = filterManager.CreateForExtendedProperty<User, long>(
                "userIdForOrgUnit",
                userIdForOrgUnit => x => x.UserOrganizationUnits
                                            .Any(y => y.OrganizationUnitDto.UserOrganizationUnits
                                                                        .Any(z => z.UserId == userIdForOrgUnit)));

            var privilegyFilter = filterManager.CreateForExtendedProperty<User, long>(
                "privilege", privilegeId => x => x.UserRoles.Select(y => y.Role).SelectMany(y => y.RolePrivileges).Distinct().Any(y => y.Privilege.Operation == privilegeId));

            // Означает, что список должен содержать только самого пользователя и его подчинённых
            var subordinatesFilter = filterManager.CreateForExtendedProperty<User, long>("subordinatesOf", userId => x => x.Id == userId);
            if (subordinatesFilter != null)
            {
                subordinatesFilter = GetSubordinatesFilter(query, subordinatesFilter);
            }

            return query
                .ApplyFilter(hideReserveUserFilter)
                .ApplyFilter(hideServiceUsersFilter)
                .ApplyFilter(roleFilter)
                .ApplyFilter(userOrgUnitFilter)
                .ApplyFilter(privilegyFilter)
                .ApplyFilter(subordinatesFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x =>
                        new ListUserDto
                            {
                                Id = x.Id,
                                DisplayName = x.DisplayName,
                                Account = x.Account,
                                FirstName = x.FirstName,
                                LastName = x.LastName,
                                DepartmentName = x.Department.Name,
                                ParentName = x.Parent.DisplayName,
                                RoleName = x.UserRoles.Select(role => role.Role.Name).OrderBy(item => item),
                                IsActive = x.IsActive,
                                IsDeleted = x.IsDeleted
                            });
        }

        private Expression<Func<User, bool>> GetSubordinatesFilter(IQueryable<User> query, Expression<Func<User, bool>> subordinatesFilter)
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