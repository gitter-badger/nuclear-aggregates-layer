using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class RevertWithdrawalOperationService : IRevertWithdrawalOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IActualizeAccountsDuringRevertingWithdrawalOperationService _actualizeAccountsDuringRevertingWithdrawalOperationService;
        private readonly IActualizeOrdersDuringRevertingWithdrawalOperationService _actualizeOrdersDuringRevertingWithdrawalOperationService;
        private readonly IActualizeDealsDuringRevertingWithdrawalOperationService _actualizeDealsDuringRevertingWithdrawalOperationService;
        private readonly IAccountWithdrawalChangeStatusAggregateService _withdrawalChangeStatusAggregateService;
        private readonly ICheckOperationPeriodService _checkOperationPeriodService;
        private readonly IAggregateServiceIsolator _aggregateServiceIsolator;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ICommonLog _logger;
        private readonly IDeleteLockDetailsDuringRevertingWithdrawalOperationService _deleteLockDetailsDuringRevertingWithdrawalOperationService;

        public RevertWithdrawalOperationService(
            IAccountReadModel accountReadModel,
            IActualizeAccountsDuringRevertingWithdrawalOperationService actualizeAccountsDuringRevertingWithdrawalOperationService,
            IActualizeOrdersDuringRevertingWithdrawalOperationService actualizeOrdersDuringRevertingWithdrawalOperationService,
            IActualizeDealsDuringRevertingWithdrawalOperationService actualizeDealsDuringRevertingWithdrawalOperationService,
            IDeleteLockDetailsDuringRevertingWithdrawalOperationService deleteLockDetailsDuringRevertingWithdrawalOperationService,
            IAccountWithdrawalChangeStatusAggregateService withdrawalChangeStatusAggregateService,
            ICheckOperationPeriodService checkOperationPeriodService,
            IAggregateServiceIsolator aggregateServiceIsolator,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IUseCaseTuner useCaseTuner,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _accountReadModel = accountReadModel;
            _actualizeAccountsDuringRevertingWithdrawalOperationService = actualizeAccountsDuringRevertingWithdrawalOperationService;
            _actualizeOrdersDuringRevertingWithdrawalOperationService = actualizeOrdersDuringRevertingWithdrawalOperationService;
            _actualizeDealsDuringRevertingWithdrawalOperationService = actualizeDealsDuringRevertingWithdrawalOperationService;
            _withdrawalChangeStatusAggregateService = withdrawalChangeStatusAggregateService;
            _checkOperationPeriodService = checkOperationPeriodService;
            _aggregateServiceIsolator = aggregateServiceIsolator;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _useCaseTuner = useCaseTuner;
            _logger = logger;
            _deleteLockDetailsDuringRevertingWithdrawalOperationService = deleteLockDetailsDuringRevertingWithdrawalOperationService;
        }

        public WithdrawalProcessingResult Revert(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod, string comment)
        {
            _useCaseTuner.AlterDuration<RevertWithdrawalOperationService>();

            WithdrawalInfo acquiredWithdrawal = null;

            try
            {
                string report;
                if (!TryAcquireTargetWithdrawal(organizationUnitId, period, accountingMethod, comment, out acquiredWithdrawal, out report))
                {
                    var msg = string.Format("Can't acquire withdrawal for organization unit id {0} by period {1}. Error: {2}",
                                            organizationUnitId,
                                            period,
                                            report);

                    _logger.ErrorFormatEx(msg);
                    return WithdrawalProcessingResult.Errors(msg);
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    if (!LockSuccessfullyAcquired(acquiredWithdrawal))
                    {
                        var msg =
                            string.Format("Acquired withdrawal with id {0} for organization unit with id {1} by period {2} has processing status violations. Possible reason for errors - concurrent withdrawal\reverting process and invalid withdrawal status processing",
                                          acquiredWithdrawal.Id,
                                          acquiredWithdrawal.OrganizationUnitId,
                                          period);

                        _logger.ErrorEx(msg);

                        transaction.Complete();
                        return WithdrawalProcessingResult.Errors(msg);
                    }

                    var result = ExecuteRevertWithdrawalProcessing(acquiredWithdrawal, organizationUnitId, period, accountingMethod);

                    transaction.Complete();

                    return result;
                }
            }
            catch (Exception ex)
            {
                var msg =
                    string.Format("Reverting withdrawing aborted. Unexpected exception was caught. Organization unit id {0}. Period: {1}",
                                  organizationUnitId,
                                  period);
                _logger.ErrorEx(ex, msg);

                if (acquiredWithdrawal != null)
                {
                    _aggregateServiceIsolator.TransactedExecute<IAccountWithdrawalChangeStatusAggregateService>(TransactionScopeOption.RequiresNew,
                                                                                                                service =>
                                                                                                                service.Finish(acquiredWithdrawal, WithdrawalStatus.Error, msg));
                }

                return WithdrawalProcessingResult.Errors(msg);
            }
        }

        private bool LockSuccessfullyAcquired(WithdrawalInfo acquiredWithdrawal)
        {
            var lockedWithdrawal =
                _accountReadModel.GetLastWithdrawal(acquiredWithdrawal.OrganizationUnitId,
                                                    new TimePeriod(acquiredWithdrawal.PeriodStartDate, acquiredWithdrawal.PeriodEndDate),
                                                    acquiredWithdrawal.AccountingMethod);
            return lockedWithdrawal != null
                   && lockedWithdrawal.Id == acquiredWithdrawal.Id
                   && lockedWithdrawal.Status == WithdrawalStatus.Reverting
                   && acquiredWithdrawal.SameVersionAs(lockedWithdrawal);
        }

        private WithdrawalProcessingResult ExecuteRevertWithdrawalProcessing(WithdrawalInfo acquiredWithdrawal, long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<WithdrawalIdentity>())
            {
                var withdrawalInfos = _accountReadModel.GetInfoForRevertWithdrawal(organizationUnitId, period, accountingMethod);

                _logger.InfoFormatEx("Reverting withdrawal. Organization unit {0}. {1}. Starting accounts actualization process. Target withdrawal infos count: {2}",
                                     organizationUnitId,
                                     period,
                                     withdrawalInfos.Length);

                _actualizeAccountsDuringRevertingWithdrawalOperationService
                    .Actualize(withdrawalInfos.Select(i => new AccountStateForRevertingWithdrawalDto
                                                               {
                                                                   Account = i.Account,
                                                                   DebitAccountDetail = i.DebitAccountDetail,
                                                                   AccountBalanceBeforeRevertingWithdrawal =
                                                                       i.AccountBalanceBeforeRevertWithdrawal,
                                                                   Lock = i.Lock,
                                                                   LockDetails = i.LockDetails
                                                               }));

                _logger.InfoFormatEx("Reverting withdrawal. Organization unit {0}. {1}. Starting orders actualization process",
                                     organizationUnitId,
                                     period);

                _actualizeOrdersDuringRevertingWithdrawalOperationService
                    .Actualize(withdrawalInfos.GroupBy(dto => dto.Order.Id, (l, dtos) => dtos.Single())
                                              .Select(dto => new ActualizeOrdersDto
                                                                 {
                                                                     Order = dto.Order,
                                                                     AmountAlreadyWithdrawn =
                                                                         dto.AmountAlreadyWithdrawnAfterWithdrawalRevert,
                                                                     AmountToWithdrawNext =
                                                                         dto.AmountToWithdrawNextAfterWithdrawalRevert
                                                                 }));

                _logger.InfoFormatEx("Reverting withdrawal. Organization unit {0}. {1}. Starting deals actualization process",
                                     organizationUnitId,
                                     period);

                _actualizeDealsDuringRevertingWithdrawalOperationService
                    .Actualize(withdrawalInfos
                                   .Where(dto => dto.Order.DealId.HasValue)
                                   .Select(dto => dto.Order.DealId.Value)
                                   .Distinct());

                _logger.InfoFormatEx("Reverting withdrawal. Organization unit {0}. {1}. Starting locks actualization process for planned positions",
                                     organizationUnitId,
                                     period);

                if (accountingMethod == AccountingMethod.PlannedProvision)
                {
                    _deleteLockDetailsDuringRevertingWithdrawalOperationService.DeleteLockDetails(organizationUnitId, period);
                }

                _withdrawalChangeStatusAggregateService.ChangeStatus(acquiredWithdrawal, WithdrawalStatus.Reverted, null);

                _logger.InfoFormatEx("Reverting withdrawal process successfully finished. Organization unit {0}. {1}.",
                                     organizationUnitId,
                                     period);

                scope.Complete();
            }

            return WithdrawalProcessingResult.Succeeded;
        }

        private bool TryAcquireTargetWithdrawal(long organizationUnitId,
                                                TimePeriod period,
                                                AccountingMethod accountingMethod,
                                                string comment,
                                                out WithdrawalInfo acquiredWithdrawal,
                                                out string report)
        {
            acquiredWithdrawal = null;

            _logger.InfoFormatEx("Starting reverting withdrawal process for organization unit with id {0} and time period {1}", organizationUnitId, period);

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, _userContext.Identity.Code))
            {
                report = "User doesn't have sufficient privileges for managing withdrawal reverting";
                return false;
            }

            if (!_checkOperationPeriodService.IsOperationPeriodValid(period, out report))
            {
                return false;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var lastWithdrawal = _accountReadModel.GetLastWithdrawal(organizationUnitId, period, accountingMethod);
                if (!CanBeReverted(lastWithdrawal, out report))
                {
                    report =
                        string.Format("Can't start reverting withdrawal process for organization unit with id {0} and time period {1}. {2}",
                                      organizationUnitId,
                                      period,
                                      report);

                    return false;
                }

                _withdrawalChangeStatusAggregateService.ChangeStatus(lastWithdrawal, WithdrawalStatus.Reverting, comment);
                acquiredWithdrawal = lastWithdrawal;
                transaction.Complete();
            }

            _logger.InfoFormatEx("Reverting withdrawal process for organization unit {0} and period {1} is granted. Acquired withdrawal entry id {2}",
                                 organizationUnitId,
                                 period,
                                 acquiredWithdrawal.Id);

            return true;
        }

        private bool CanBeReverted(WithdrawalInfo withdrawal, out string report)
        {
            report = null;

            if (withdrawal == null)
            {
                report = BLResources.NotFoundAnySuccesfulWithdrawalForOrgUnitAndPeriod;
                return false;
            }

            bool canBeReverted = false;
            switch (withdrawal.Status)
            {
                case WithdrawalStatus.Withdrawing:
                {
                    report = BLResources.CannotRevertWithdrawalBecauseAnotherWithdrawalIsRunning;
                    break;
                }

                case WithdrawalStatus.Reverting:
                {
                    report = BLResources.CannotRevertWithdrawalBecauseAnotherWithdrawalIsRunning;
                    break;
                }

                case WithdrawalStatus.Error:
                {
                    report = EnumResources.CannotRevertWithdrawalBecausePreviousWithdrawalIsFailed;
                    break;
                }

                case WithdrawalStatus.Reverted:
                {
                    report = EnumResources.WithdrawalIsAlreadyReverted;
                    break;
                }

                case WithdrawalStatus.Success:
                {
                    canBeReverted = true;
                    break;
                }
            }

            return canBeReverted;
        }
    }
}