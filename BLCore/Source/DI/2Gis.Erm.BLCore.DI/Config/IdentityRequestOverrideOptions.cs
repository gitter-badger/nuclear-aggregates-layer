using System;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    [Flags]
    public enum IdentityRequestOverrideOptions
    {
        None = 0,
        UseNullRequestChecker = 1,
        UseNullRequestStrategy = 2
    }
}