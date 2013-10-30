using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.API.WCF.MoDi.Settings
{
    public sealed class MoDiAppSettings : CommonConfigFileAppSettings, IMoDiAppSettings
    {
        public IMsCrmSettings MsCrmSettings
        {
            get { return MsCRMSettings; }
        }
    }
}