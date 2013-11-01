using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.WCF.Metadata.Settings;

namespace DoubleGis.Erm.API.WCF.Metadata.Settings
{
    public sealed class MetadataServiceAppSettings : CommonConfigFileAppSettings, IMetadataServiceAppSettings
    {
        private readonly IntSetting _identityServiceUniqueId = ConfigFileSetting.Int.Required("IdentityServiceUniqueId");
        public int IdentityServiceUniqueId
        {
            get
            {
                return _identityServiceUniqueId.Value;
            }
        }

        public IMsCrmSettings MsCrmSettings
        {
            get { return MsCRMSettings; }
        }

        public APIServicesSettingsAspect ServicesSettings { get { return APIServicesSettings; } }
    }
}