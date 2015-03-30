using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ActualizeAccountsDuringRevertingWithdrawalOperationService : IActualizeAccountsDuringRevertingWithdrawalOperationService
    {
        private readonly IAccountRevertWithdrawFromAccountsAggregateService _accountRevertWithdrawFromAccountsAggregateService;
        private readonly IAccountBulkActivateLocksAggregateService _accountBulkActivateLocksAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public ActualizeAccountsDuringRevertingWithdrawalOperationService(
            IAccountRevertWithdrawFromAccountsAggregateService accountRevertWithdrawFromAccountsAggregateService,
            IAccountBulkActivateLocksAggregateService accountBulkActivateLocksAggregateService,
            IUseCaseTuner useCaseTuner, 
            IOperationScopeFactory scopeFactory, 
            ITracer tracer)
        {
            _accountRevertWithdrawFromAccountsAggregateService = accountRevertWithdrawFromAccountsAggregateService;
            _accountBulkActivateLocksAggregateService = accountBulkActivateLocksAggregateService;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public void Actualize(IEnumerable<AccountStateForRevertingWithdrawalDto> accountInfos)
        {
            _useCaseTuner.AlterDuration<ActualizeAccountsDuringWithdrawalOperationService>();

            _tracer.InfoFormat("Starting accounts state actualization process during reverting withdrawal");

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeAccountsDuringRevertingWithdrawalIdentity>())
            {
                _accountRevertWithdrawFromAccountsAggregateService
                        .RevertWithdraw(
                            accountInfos.GroupBy(
                                dto => dto.Account.Id,
                                (l, dtos) =>
                                    {
                                        var accountInfo = dtos.First();
                                        return new RevertWithdrawFromAccountsDto
                                            {
                                                Account = accountInfo.Account,
                                                BalanceBeforeRevertWithdrawal = accountInfo.AccountBalanceBeforeRevertingWithdrawal,
                                                DebitDetails = dtos.Select(dto => dto.DebitAccountDetail)
                                            };
                                    }));

                _accountBulkActivateLocksAggregateService
                    .Activate(accountInfos.Select(x => 
                        new ActivateLockDto
                            {
                                Lock = x.Lock, 
                                Details = x.LockDetails
                            }));

                scope.Complete();
            }

            _tracer.InfoFormat("Finished accounts state actualization process during reverting withdrawal");
        }
    }
}