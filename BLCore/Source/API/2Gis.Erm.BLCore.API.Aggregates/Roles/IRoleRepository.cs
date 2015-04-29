using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles
{
    public interface IRoleRepository : IAggregateRootRepository<Role>,
                                       IDeleteAggregateRepository<Role>
    {
        int Delete(Role role);
        void CreateOrUpdate(Role role);

        IEnumerable<EntityPrivilegeInfo> GetEntityPrivileges(long roleId);
        IEnumerable<FunctionalPrivilegeInfo> GetFunctionalPrivileges(long roleId);

        void UpdateFunctionalPrivileges(long roleId, FunctionalPrivilegeInfo[] privilegeInfos);
        void UpdateEntityPrivileges(long roleId, PrivilegeDto[] privilegeInfos);

        bool HasUsers(long roleId);
        IEnumerable<FunctionalPrivilegeInfo> FindAllFunctionalPriveleges();
    }
}