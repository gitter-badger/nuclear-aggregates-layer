using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    public interface IAccountDebtsChecker : IInvariantSafeCrosscuttingService
    {
        bool HasDebts(bool bypassValidation, long userCode, Func<IReadOnlyCollection<long>> getTargetAccountsFunc, out string message);
        bool HasDebts(bool bypassValidation, long userCode, Func<IReadOnlyCollection<long>> getTargetAccountsFunc, Action<IReadOnlyCollection<AccountWithDebtInfo>> processErrorsAction, out string message);
    }
}