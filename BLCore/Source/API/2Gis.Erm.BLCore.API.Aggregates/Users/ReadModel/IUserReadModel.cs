using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel.DTO;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Security;

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
        long? GetUserOrganizationUnitId(long userId);
        IReadOnlyDictionary<long, string> GetUserNames(IEnumerable<long> userIds);
        string GetUserName(long userId);
        bool IsUserLinkedToBranchOffice(long userId, long branchOffice);
        IReadOnlyCollection<long> GetUserBranchOffices(long userId);
        IReadOnlyCollection<UserBranchOffice> GetUserBranchOfficeLinks(long userId);
        IEnumerable<long> PickNonServiceUsers(IEnumerable<long> userIds);
        bool CheckIfUserAndBranchOfficeHaveCommonOrganizationUnit(long userId, long branchOfficeId);
    }
}