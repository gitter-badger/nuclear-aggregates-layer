using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Roles
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