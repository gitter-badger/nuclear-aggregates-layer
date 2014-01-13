using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Roles
{
    public class RoleSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Role> ById(long id)
            {
                return new FindSpecification<Role>(x => x.Id == id);
            }

            public static FindSpecification<UserRole> UserRoles(long roleId)
            {
                return new FindSpecification<UserRole>(x => x.RoleId == roleId);
            }
        }
    }
}