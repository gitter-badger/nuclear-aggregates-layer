using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel
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