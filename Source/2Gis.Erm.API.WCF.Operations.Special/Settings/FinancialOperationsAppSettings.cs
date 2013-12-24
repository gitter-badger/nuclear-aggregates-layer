using DoubleGis.Erm.BL.WCF.Operations.Special.FinancialOperations.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.API.WCF.Operations.Special.Settings
{
    public sealed class FinancialOperationsAppSettings : CommonConfigFileAppSettings, IFinancialOperationsAppSettings
    {
        private readonly BoolSetting _creationOrderRequestsEnabled = ConfigFileSetting.Bool.Required("CreationOrderRequestsEnabled");

        public APIServicesSettingsAspect ServicesSettings
        {
            get { return APIServicesSettings; }
        }

        public IMsCrmSettings MsCrmSettings
        {
            get { return MsCRMSettings; }
        }

    }
}