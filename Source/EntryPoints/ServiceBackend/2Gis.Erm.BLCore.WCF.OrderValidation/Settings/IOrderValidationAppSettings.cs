using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.BLCore.WCF.OrderValidation.Settings
{
    public interface IOrderValidationAppSettings : 
        IAppSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
    }
}