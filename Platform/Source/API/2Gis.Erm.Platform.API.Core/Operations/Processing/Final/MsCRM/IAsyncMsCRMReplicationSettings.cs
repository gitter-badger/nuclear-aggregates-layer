using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM
{
    public interface IAsyncMsCRMReplicationSettings : ISettings
    {
        int ReplicationTimeoutSec { get; }
    }
}