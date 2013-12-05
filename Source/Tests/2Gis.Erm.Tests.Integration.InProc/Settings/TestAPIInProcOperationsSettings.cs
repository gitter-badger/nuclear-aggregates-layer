using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.Tests.Integration.InProc.Settings
{
    public sealed class TestAPIInProcOperationsSettings : CommonConfigFileAppSettings, ITestAPIInProcOperationsSettings
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