using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.TaskService.Settings
{
    public interface ITaskServiceProcessingSettings : ISettings
    {
        int MaxWorkingThreads { get; }
        JobStoreType JobStoreType { get; }
        string SchedulerName { get; }
    }
}