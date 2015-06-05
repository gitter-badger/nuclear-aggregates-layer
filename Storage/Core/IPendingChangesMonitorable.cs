namespace NuClear.Storage.Core
{
    public interface IPendingChangesMonitorable
    {
        bool AnyPendingChanges { get; } 
    }
}