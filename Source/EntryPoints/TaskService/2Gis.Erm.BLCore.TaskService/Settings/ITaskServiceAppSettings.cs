using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.TaskService.Settings;

namespace DoubleGis.Erm.BLCore.TaskService.Settings
{
    public interface ITaskServiceAppSettings : 
        IAppSettings,
        INotificationProcessingSettings,
        IIntegrationSettings,
        IIntegrationLocalizationSettings,
        IGetUserInfoFromAdSettings,
        IDBCleanupSettings,
        ITaskServiceProcesingSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
    }
}