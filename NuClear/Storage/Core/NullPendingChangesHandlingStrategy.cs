namespace NuClear.Storage.Core
{
    /// <summary>
    /// Реализация PendingChangesHandlingStrategy по-умолчанию. Отложенные изменения не контролируются
    /// </summary>
    public class NullPendingChangesHandlingStrategy : IPendingChangesHandlingStrategy
    {
        public void HandlePendingChanges(IPendingChangesMonitorable pendingChangesMonitorableObject)
        {
        }
    }
}