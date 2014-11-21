namespace DoubleGis.Erm.Platform.DAL
{
    public interface IPendingChangesHandlingStrategy
    {
        void HandlePendingChanges(IPendingChangesMonitorable pendingChangesMonitorableObject);
    }
}