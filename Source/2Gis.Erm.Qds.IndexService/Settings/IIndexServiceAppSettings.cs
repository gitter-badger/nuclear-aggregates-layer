using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.Qds.IndexService.Settings
{
    public interface IIndexServiceAppSettings :
        IAppSettings,
        INotificationProcessingSettings,
        IIntegrationSettings,
        IIntegrationLocalizationSettings,
        IGetUserInfoFromAdSettings,
        IDBCleanupSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
        BatchIndexingSettings BatchIndexingSettings { get; }
    }
}