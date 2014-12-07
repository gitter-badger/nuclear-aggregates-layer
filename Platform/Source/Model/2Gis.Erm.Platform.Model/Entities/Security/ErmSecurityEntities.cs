using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public static class ErmSecurityEntities
    {
        public static Type[] Entities =
        {
            typeof(Department),
            typeof(FunctionalPrivilegeDepth),
            typeof(OrganizationUnitDto),
            typeof(Privilege),
            typeof(Role),
            typeof(RolePrivilege),
            typeof(TerritoryDto),
            typeof(TimeZone),
            typeof(User),
            typeof(UserEntity),
            typeof(UserOrganizationUnit),
            typeof(UserProfile),
            typeof(UserRole),
            typeof(UserTerritory)
        };
    }
}