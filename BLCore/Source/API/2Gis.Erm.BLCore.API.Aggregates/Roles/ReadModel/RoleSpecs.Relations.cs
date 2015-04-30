using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.ReadModel
{
    public static class RoleSpecs
    {
        public static class Relations
        {
            public static class Find
            {
                public static FindSpecification<UserRole> UserRoles(long roleId)
                {
                    return new FindSpecification<UserRole>(x => x.RoleId == roleId);
                }
            }
        }
    }
}