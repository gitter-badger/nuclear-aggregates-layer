﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ActualizeAccountsDuringWithdrawalOperationService : IActualizeAccountsDuringWithdrawalOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IAccountWithdrawFromAccountsAggregateService _accountWithdrawFromAccountsAggregateService;
        private readonly IAccountBulkDeactivateUsedLockAggregateService _accountBulkDeactivateUsedLockAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public ActualizeAccountsDuringWithdrawalOperationService(
            IAccountReadModel accountReadModel,
            IAccountWithdrawFromAccountsAggregateService accountWithdrawFromAccountsAggregateService,
            IAccountBulkDeactivateUsedLockAggregateService accountBulkDeactivateUsedLockAggregateService,
            IUseCaseTuner useCaseTuner, 
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _accountReadModel = accountReadModel;
            _accountWithdrawFromAccountsAggregateService = accountWithdrawFromAccountsAggregateService;
            _accountBulkDeactivateUsedLockAggregateService = accountBulkDeactivateUsedLockAggregateService;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Actualize(TimePeriod withdrawalPeriod, IEnumerable<AccountStateForWithdrawalDto> accountInfos)
        {
            _useCaseTuner.AlterDuration<ActualizeAccountsDuringWithdrawalOperationService>();

            _logger.InfoFormatEx("Starting accounts state actualization process during withdrawal");

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeAccountsDuringWithdrawalIdentity>())
            {
                var debitForOrderPaymentOperationTypeId = _accountReadModel.ResolveDebitForOrderPaymentOperationTypeId();

                var debitAccountDetailsMap =
                    _accountWithdrawFromAccountsAggregateService
                        .Withdraw(accountInfos.Select(x => 
                            new WithdrawFromAccountsDto
                            {
                                Account = x.Account,
                                BalanceBeforeWithdrawal = x.AccountBalanceBeforeWithdrawal,
                                LockBalance = x.LockBalance,
                                LockId = x.Lock.Id,
                                OrderNumber = x.OrderNumber
                            }), 
                            debitForOrderPaymentOperationTypeId, 
                            withdrawalPeriod.Start);

                _accountBulkDeactivateUsedLockAggregateService
                    .Deactivate(accountInfos.Select(x =>
                            new DeactivateLockDto
                            {
                                Lock = x.Lock,
                                Details = x.Details,
                                LockBalance = x.LockBalance
                            }), 
                            debitAccountDetailsMap);

                scope.Complete();
            }

            _logger.InfoFormatEx("Finished accounts state actualization process during withdrawal");
        }
    }
}