using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM
{
    public sealed class AsyncMsCRMReplicationSettingsAspect : ISettingsAspect, IAsyncMsCRMReplicationSettings
    {
        private readonly IntSetting _asyncMsCRMReplicationTimeoutSec = ConfigFileSetting.Int.Optional("AsyncMsCRMReplicationTimeoutSec", 60);

        public int ReplicationTimeoutSec
        {
            get { return _asyncMsCRMReplicationTimeoutSec.Value; }
        }
    }
}