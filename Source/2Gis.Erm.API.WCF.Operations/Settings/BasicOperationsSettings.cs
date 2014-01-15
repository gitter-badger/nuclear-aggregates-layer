using DoubleGis.Erm.BLCore.WCF.Operations.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.WCF.BasicOperations.Settings
{
    public sealed class BasicOperationsSettings : CommonConfigFileAppSettings, IBasicOperationsSettings
    {
        private readonly IntSetting _progressCallbackBatchSize = ConfigFileSetting.Int.Optional("ProgressCallbackBatchSize", 1);
        public int ProgressCallbackBatchSize
        {
            get
            {
                return _progressCallbackBatchSize.Value;
            }
        }

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