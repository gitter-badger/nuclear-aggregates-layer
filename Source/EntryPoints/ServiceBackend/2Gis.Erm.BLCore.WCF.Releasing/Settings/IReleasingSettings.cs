using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.BLCore.WCF.Releasing.Settings
{
    public interface IReleasingSettings : 
        IAppSettings, 
        IIntegrationSettings, 
        IFtpExportIntegrationModeSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
    }
}