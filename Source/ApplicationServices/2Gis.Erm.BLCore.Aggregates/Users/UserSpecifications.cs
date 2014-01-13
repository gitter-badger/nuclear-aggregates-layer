using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Users
{
    public static class UserSpecifications
    {
        public static class Find
        {
            public static FindSpecification<User> ById(long id)
            {
                return new FindSpecification<User>(x => x.Id == id);
            }
            public static FindSpecification<UserRole> UserRoles(long userId)
            {
                return new FindSpecification<UserRole>(x => x.UserId == userId);
            }

            public static FindSpecification<User> UsersInDepartment(long departmentId)
            {
                return new FindSpecification<User>(x => !x.IsDeleted && x.DepartmentId == departmentId);
            }

            public static FindSpecification<User> AllUsersByDepartments(IEnumerable<long> departmentIds)
            {
                return new FindSpecification<User>(u => departmentIds.Contains(u.DepartmentId));
            }

            public static FindSpecification<UserProfile> ActiveProfilesForUser(long userCode)
            {
                return new FindSpecification<UserProfile>(up => up.UserId == userCode && !up.IsDeleted && up.IsActive);
            }

            public static FindSpecification<User> ActiveNotService()
            {
                return new FindSpecification<User>(x => x.IsActive && !x.IsDeleted && !x.IsServiceUser);
            }
        }
    }
}