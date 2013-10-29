using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings
{
    /// <summary>
    /// Интерфейс для доступа к глобальным настройкам приложения
    /// </summary>
    public interface IAppSettings : IGlobalizationSettings, IConnectionStringSettingsHost
    {
        string ReserveUserAccount { get; }
        bool EnableNotifications { get; }
        bool EnableCaching { get; }

        int SignificantDigitsNumber { get; }

        decimal MinDebtAmount { get; }
        int WarmClientDaysCount { get; }

        int OrderRequestProcessingHoursAmount { get; }

        AppTargetEnvironment TargetEnvironment { get; }
        string EntryPointName { get; }
    }
}
