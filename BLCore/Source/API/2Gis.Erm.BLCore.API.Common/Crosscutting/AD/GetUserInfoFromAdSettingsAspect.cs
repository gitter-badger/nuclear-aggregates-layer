using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD
{
    public sealed class GetUserInfoFromAdSettingsAspect : ISettingsAspect, IGetUserInfoFromAdSettings
    {
        private readonly StringSetting _adConnectionDomainName = ConfigFileSetting.String.Required("ADConnectionDomainName");
        private readonly StringSetting _adConnectionLogin = ConfigFileSetting.String.Required("ADConnectionLogin");
        private readonly StringSetting _adConnectionPassword = ConfigFileSetting.String.Required("ADConnectionPassword");

        public string ADConnectionDomainName
        {
            get
            {
                return _adConnectionDomainName.Value;
            }
        }

        public string ADConnectionLogin
        {
            get
            {
                return _adConnectionLogin.Value;
            }
        }

        public string ADConnectionPassword
        {
            get
            {
                return _adConnectionPassword.Value;
            }
        }
    }
}