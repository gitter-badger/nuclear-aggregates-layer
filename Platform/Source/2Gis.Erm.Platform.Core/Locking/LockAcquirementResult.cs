namespace DoubleGis.Erm.Platform.Core.Locking
{
    public enum LockAcquirementResult
    {
        GrantedSynchronously = 0,
        GrantedAfterWaiting = 1,
        RequestTimeout = -1,
        RequestWasCanceled = -2,
        RequestWasChosenAsADeadlockVictim = -3,
        OtherError = -999
    }
}