using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Users.ReadModel
{
    public static partial class UserSpecs
    {
        public static class UserProfiles
        {
            public static class Find
            {
                public static FindSpecification<UserProfile> ForUser(long userCode)
                {
                    return new FindSpecification<UserProfile>(up => up.UserId == userCode);
                }
            }
        }
    }
}