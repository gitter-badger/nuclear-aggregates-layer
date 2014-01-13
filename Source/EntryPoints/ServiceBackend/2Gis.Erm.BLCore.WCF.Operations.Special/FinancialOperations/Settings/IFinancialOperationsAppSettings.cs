using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.FinancialOperations.Settings
{
    public interface IFinancialOperationsAppSettings : 
        IAppSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
    }
}