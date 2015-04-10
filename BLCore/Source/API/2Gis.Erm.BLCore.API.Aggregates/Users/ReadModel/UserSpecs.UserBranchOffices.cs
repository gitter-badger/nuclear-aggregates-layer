using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel
{
    public static partial class UserSpecs
    {
        public static class UserBranchOffices
        {
            public static class Find
            {
                public static FindSpecification<UserBranchOffice> ByUser(long userId)
                {
                    return new FindSpecification<UserBranchOffice>(x => x.UserId == userId);
                }
            }
        }
    }
}