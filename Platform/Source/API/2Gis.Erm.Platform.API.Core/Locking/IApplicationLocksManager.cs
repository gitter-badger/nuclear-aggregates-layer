namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface IApplicationLocksManager
    {
        void Release(ITrackedLockingScope scope);
    }
}