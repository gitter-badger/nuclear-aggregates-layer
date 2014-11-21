using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services
{
    public interface IUserProfileDataProvider
    {
        /// <summary>
        /// Набор настроек доступных для пользователя
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <returns></returns>
        LocalSettingsDto GetLocalSettings(long userCode);

        /// <summary>
        /// Набор настроек доступных для пользователя
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <returns></returns>
        UserPersonalInfoDto GetPersonalInfo(long userCode);

        /// <summary>
        /// Набор настроек региональных параметров, поддреживаемый данной инсталляцией ERM
        /// </summary>
        /// <returns></returns>
        SupportedLocalSettingsDto SupportedLocalSettings { get; }

        /// <summary>
        /// Сохранить информацию профиля пользователя
        /// </summary>
        UserProfileViewModel SaveUserProfileInfo(UserProfileViewModel serverViewModel);
    }
}
