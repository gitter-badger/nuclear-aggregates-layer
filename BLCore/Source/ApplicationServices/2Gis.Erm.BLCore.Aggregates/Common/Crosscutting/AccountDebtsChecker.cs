using System;
using System.Collections.Generic;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public class AccountDebtsChecker : IAccountDebtsChecker
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IAccountReadModel _accountReadModel;

        public AccountDebtsChecker(ISecurityServiceFunctionalAccess functionalAccessService, IAccountReadModel accountReadModel)
        {
            _functionalAccessService = functionalAccessService;
            _accountReadModel = accountReadModel;
        }

        public void Check(bool bypassValidation,
                          long userCode,
                          Func<IReadOnlyCollection<long>> getTargetAccountsFunc,
                          Action<IReadOnlyCollection<AccountWithDebtInfo>> processErrorsAction)
        {
            if (bypassValidation)
            {
                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, userCode))
                {
                    throw new SecurityException(BLResources.ProcessAccountsWithDebtsOperationIsNotAllowed);
                }
            }
            else
            {
                var accountsWithDebts = _accountReadModel.GetAccountsWithDebts(getTargetAccountsFunc());
                processErrorsAction(accountsWithDebts);
                CheckForDebtsHelper.ThrowIfAnyError(accountsWithDebts);
            }
        }
    }
}