using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.Settings
{
    public interface INotifiyProgressSettings : ISettings
    {
        int ProgressCallbackBatchSize { get; }
    }
}