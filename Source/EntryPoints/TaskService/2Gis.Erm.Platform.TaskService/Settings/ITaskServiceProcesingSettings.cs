using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.TaskService.Settings
{
    public interface ITaskServiceProcesingSettings : ISettings
    {
        int MaxWorkingThreads { get; }
    }
}