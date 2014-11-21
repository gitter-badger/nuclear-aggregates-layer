using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto
{
    public sealed class UserProfileDto
    {
        public string UserAccountName { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}