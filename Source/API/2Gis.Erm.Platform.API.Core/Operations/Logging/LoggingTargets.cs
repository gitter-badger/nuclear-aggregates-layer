using System;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    [Flags]
    public enum LoggingTargets
    {
        None = 0,
        DB = 1,
        Queue = 2
    }
}
