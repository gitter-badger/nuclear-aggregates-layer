using DoubleGis.Erm.BL.WCF.Operations.Special.FinancialOperations.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;

namespace DoubleGis.Erm.API.WCF.Operations.Special.Settings
{
    public sealed class FinancialOperationsAppSettings : CommonConfigFileAppSettings, IFinancialOperationsAppSettings
    {
        public APIServicesSettingsAspect ServicesSettings
        {
            get { return APIServicesSettings; }
        }
    }
}