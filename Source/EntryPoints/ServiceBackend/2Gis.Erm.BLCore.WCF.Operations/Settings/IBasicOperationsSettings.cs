using DoubleGis.Erm.BLCore.API.Operations.Remote;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Settings
{
    public interface IBasicOperationsSettings :
        IAppSettings,
        INotifiyProgressSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
    }
}