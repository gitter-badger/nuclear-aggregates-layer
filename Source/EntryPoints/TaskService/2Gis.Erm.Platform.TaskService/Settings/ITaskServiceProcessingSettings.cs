using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.TaskService.Settings
{
    public interface ITaskServiceProcessingSettings : ISettings
    {
        int MaxWorkingThreads { get; }
        JobStoreType JobStoreType { get; }
        string SchedulerName { get; }
    }
}