using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.Tests.Integration.InProc.Settings
{
    public interface ITestAPIInProcOperationsSettings :
        IAppSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
    }
}