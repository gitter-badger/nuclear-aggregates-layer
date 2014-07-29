using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public partial class UserProfileViewModel
    {
        public long UserId { get; set; }
        public string DomainAccountName { get; set; }
        public UserPersonalInfoDto PersonalInfo { get; set; }
        public LocalSettingsDto Localsettings { get; set; }
    }
}