using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.PersistenceCleanup
{
    public interface IDBCleanupSettings : ISettings
    {
        int LogSizeInDays { get; }
    }
}
