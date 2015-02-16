using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel
{
    public static partial class UserSpecs
    {
        public static class Users
        {
            public static class Find
            {
                public static FindSpecification<User> ByDepartment(long departmentId)
                {
                    return new FindSpecification<User>(x => x.DepartmentId == departmentId);
                }

                public static FindSpecification<User> ByDepartments(IEnumerable<long> departmentIds)
                {
                    return new FindSpecification<User>(u => departmentIds.Contains(u.DepartmentId));
                }

                public static FindSpecification<User> NotService()
                {
                    return new FindSpecification<User>(x => !x.IsServiceUser);
                }

                public static FindSpecification<User> ByRole(long roleId)
                {
                    return new FindSpecification<User>(x => x.UserRoles.Any(ur => ur.RoleId == roleId));
                }
            }
        }
    }
}