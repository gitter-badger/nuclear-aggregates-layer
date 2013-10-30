using DoubleGis.Erm.BL.WCF.OrderValidation.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.API.WCF.OrderValidation.Settings
{
    public sealed class OrderValidationAppSettings : CommonConfigFileAppSettings, IOrderValidationAppSettings
    {
        public IMsCrmSettings MsCrmSettings
        {
            get { return MsCRMSettings; }
        }

        public APIServicesSettingsAspect ServicesSettings
        {
            get { return APIServicesSettings; }
        }
    }
}