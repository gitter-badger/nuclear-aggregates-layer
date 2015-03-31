using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using NuClear.Security.API.UserContext.Profile;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using OrganizationUnitDto = DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto.OrganizationUnitDto;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users
{
    public interface IUserRepository : IAggregateRootRepository<User>,
                                       IActivateAggregateRepository<Department>,
                                       IActivateAggregateRepository<OrganizationUnit>,
                                       IDeactivateAggregateRepository<OrganizationUnit>,
                                       IDeactivateAggregateRepository<Territory>,
                                       IDeactivateAggregateRepository<Department>,
                                       IDeleteAggregateRepository<OrganizationUnit>,
                                       IDeleteAggregateRepository<UserOrganizationUnit>
    {
        int Activate(Department department);
        int Activate(OrganizationUnit organizationUnit);
        int Deactivate(OrganizationUnit organizationUnit);
        int Deactivate(Territory territory);
        int Deactivate(Department department);
        int Delete(OrganizationUnit organizationUnit);

        int Delete(UserRole userRole);
        int Delete(UserOrganizationUnit userOrganizationUnit);

        int DeleteUserRole(long userId, long roleId);
        int DeleteUserOrganizationUnit(long userId, long organizationUnitId);

        IEnumerable<User> GetUsersByDepartments(IEnumerable<long> departmentIds);
        IEnumerable<User> GetUsersByTerritory(long territoryId);
        OrganizationUnit GetOrganizationUnit(long orgUnitId);
        OrganizationUnitWithUsersDto GetOrganizationUnitDetails(long entityId);
        OrganizationUnitDto GetSingleOrDefaultOrganizationUnit(long userId);
        IEnumerable<User> GetUsersByOrganizationUnit(long organizationUnitId);

        /// <summary>Получить активные территории, на которые завязан непосредственно указанный пользователь.</summary>
        IEnumerable<long> GetUserTerritoryIds(long userId);

        /// <summary>Получить активные территории, привязанные к отделениям 2гис, в которых состоит пользователь.</summary>
        IEnumerable<long> GetUserOrganizationUnitsTerritoryIds(long userId);

        /// <summary>Получить все активные территории, без учёта привязки к пользователю.</summary>
        IEnumerable<long> GetAllTerritoryIds();

        void CreateOrUpdate(User user);
        void CreateOrUpdate(Department department);

        void CreateOrUpdate(UserOrganizationUnit userOrganizationUnit);

        void CreateOrUpdate(UserTerritory userTerritory);

        UserProfile GetProfileForUser(long userCode);
        UserProfileDto[] GetAllUserProfiles();
        void UpdateUserProfiles(UserProfileDto[] userProfileDtos);
        void CreateOrUpdate(UserProfile profile);

        LocaleInfo GetUserLocaleInfo(long userCode);

        bool IsUserLinkedWithOrganizationUnit(long userId, long organizationUnitId);

        void CreateOrUpdate(Territory territory);
        Territory GetTerritory(long territoryId);

        void CreateOrUpdate(OrganizationUnit organizationUnit);

        void AssignUserRelatedEntities(long userId, long newOwnerCode);

        bool TryGetSingleUserOrganizationUnit(long userId, out OrganizationUnit organizationUnit);

        OrganizationUnit GetFirstUserOrganizationUnit(long userId);

        void ChangeUserTerritory(IEnumerable<User> users, long oldTerritoryId, long newTerritoryId);

        IEnumerable<CategoryGroup> GetCategoryGroups();
        IEnumerable<CategoryGroupMembershipDto> GetCategoryGroupMembership(long organizationUnitId);
    }
}