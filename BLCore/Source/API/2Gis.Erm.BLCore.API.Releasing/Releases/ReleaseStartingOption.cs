using System;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    [Flags]
    public enum ReleaseStartingOption
    {
        Undifined = 0,
        
        Denied = 1,
        BecauseOfFinal = 2,
        BecauseOfLock = 4,
        BecauseOfInconsistency = 8,
        
        Allowed = 16,
        New = 32,
        AsPrevious = 64
    }
}