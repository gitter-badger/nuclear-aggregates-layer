namespace DoubleGis.Erm.Platform.DAL
{
    public interface IPendingChangesMonitorable
    {
        bool AnyPendingChanges { get; } 
    }
}