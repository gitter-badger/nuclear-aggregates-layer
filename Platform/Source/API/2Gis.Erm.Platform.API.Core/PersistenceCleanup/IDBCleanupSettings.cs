using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.PersistenceCleanup
{
    public interface IDBCleanupSettings : ISettings
    {
        int LogSizeInDays { get; }
    }
}
