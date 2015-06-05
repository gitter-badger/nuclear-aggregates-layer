namespace NuClear.Storage.Core
{
    public interface IPendingChangesHandlingStrategy
    {
        void HandlePendingChanges(IPendingChangesMonitorable pendingChangesMonitorableObject);
    }
}