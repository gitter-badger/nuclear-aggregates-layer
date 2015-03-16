using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    // возможно должно быть operation
    public interface IAccountDebtsChecker : IInvariantSafeCrosscuttingService
    {
        void Check(bool bypassValidation, long userCode, Func<IReadOnlyCollection<long>> getTargetAccountsFunc, Action<IReadOnlyCollection<AccountWithDebtInfo>> processErrorsAction);
    }
}