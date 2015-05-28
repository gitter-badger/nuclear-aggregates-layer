using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel.DTO;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel
{
    public interface IUserReadModel : IAggregateReadModel<User>
    {
        User GetUser(long id);
        UserProfile GetProfileForUser(long userid);
        UserWithRoleRelationsDto GetUserWithRoleRelations(long userid);
        User FindAnyUserWithPrivelege(IEnumerable<long> organizationUnitId, FunctionalPrivilegeName privelege);
        User GetNotServiceUser(long userId);
        User GetOrganizationUnitDirector(long organizationUnitId);
        Uri GetTelephonyServerAddress(long userId);
        long? GetUserOrganizationUnitId(long userId);
        IReadOnlyDictionary<long, string> GetUserNames(IEnumerable<long> userIds);
        IEnumerable<long> PickNonServiceUsers(IEnumerable<long> userIds);
    }
}