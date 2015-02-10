using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel.DTO;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Users.ReadModel
{
    public sealed class UserReadModel : IUserReadModel
    {
        private readonly IFinder _finder;

        public UserReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public User GetUser(long id)
        {
            return _finder.Find(Specs.Find.ById<User>(id)).Single();
        }

        public UserProfile GetProfileForUser(long userid)
        {
            return _finder.Find(UserSpecs.UserProfiles.Find.ForUser(userid)).SingleOrDefault();
        }

        public UserWithRoleRelationsDto GetUserWithRoleRelations(long userid)
        {
            return _finder.Find(Specs.Find.ById<User>(userid))
                            .Select(x => new UserWithRoleRelationsDto
                                        {
                                            User = x,
                                            RolesRelations = x.UserRoles
                                        })
                            .Single();
        }

        public User FindAnyUserWithPrivelege(IEnumerable<long> organizationUnitId, FunctionalPrivilegeName privelegeName)
        {
            // TODO {a.rechkalov, 25.11.2013}: тут можно использовать спецификации
            var rolesWithPrivelege = _finder.Find<Privilege>(privilege => privilege.Operation == (int)privelegeName)
                                            .SelectMany(privilege => privilege.RolePrivileges)
                                            .Select(link => link.RoleId);

            var usersOfOrganizationUnit = _finder.Find<UserOrganizationUnit>(unit => organizationUnitId.Contains(unit.OrganizationUnitId))
                                                 .Select(unit => unit.User);

            return usersOfOrganizationUnit.FirstOrDefault(user => user.UserRoles.Any(role => rolesWithPrivelege.Contains(role.RoleId)));
        }

        public User GetNotServiceUser(long userId)
        {
            return _finder.Find(Specs.Find.ById<User>(userId)
                                    && UserSpecs.Users.Find.NotService()
                                    && Specs.Find.ActiveAndNotDeleted<User>())
                          .SingleOrDefault();
        }

        public User GetOrganizationUnitDirector(long organizationUnitId)
        {
            const int DirectorRoleId = 2;

            return _finder.Find(Specs.Find.ActiveAndNotDeleted<User>()
                                    && UserSpecs.Users.Find.NotService())
                          .FirstOrDefault(user => user.UserRoles.Any(role => role.RoleId == DirectorRoleId)
                                                  && user.UserOrganizationUnits.Any(unit => unit.OrganizationUnitId == organizationUnitId));
        }

        public long? GetUserOrganizationUnitId(long userId)
        {
            var singleOrganizationUnitIds = _finder.Find(Specs.Find.ById<User>(userId))
                .SelectMany(x => x.UserOrganizationUnits)
                .Select(x => x.OrganizationUnitId)
                .Take(2)
                .ToArray();

            if (singleOrganizationUnitIds.Length == 1)
            {
                return singleOrganizationUnitIds.Single();
            }

            return null;
        }
    }
}