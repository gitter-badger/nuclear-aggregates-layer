using NuClear.Settings.API;

namespace NuClear.Jobs.Settings
{
    public interface ITaskServiceProcessingSettings : ISettings
    {
        int MaxWorkingThreads { get; }
        JobStoreType JobStoreType { get; }
        string SchedulerName { get; }
    }
}