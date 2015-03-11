using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal;
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
    public sealed class WithdrawalOperationService : IWithdrawalOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IActualizeAccountsDuringWithdrawalOperationService _actualizeAccountsDuringWithdrawalOperationService;
        private readonly IActualizeOrdersDuringWithdrawalOperationService _actualizeOrdersDuringWithdrawalOperationService;
        private readonly IActualizeDealsDuringWithdrawalOperationService _actualizeDealsDuringWithdrawalOperationService;
        private readonly IAccountWithdrawalStartAggregateService _withdrawalStartAggregateService;
        private readonly IAccountWithdrawalChangeStatusAggregateService _withdrawalChangeStatusAggregateService;
        private readonly ICheckOperationPeriodService _checkOperationPeriodService;
        private readonly IAggregateServiceIsolator _aggregateServiceIsolator;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;
        private readonly ICreateLockDetailsDuringWithdrawalOperationService _createLockDetailsDuringWithdrawalOperationService;

        public WithdrawalOperationService(
            IAccountReadModel accountReadModel,
            ICreateLockDetailsDuringWithdrawalOperationService createLockDetailsDuringWithdrawalOperationService,
            IActualizeAccountsDuringWithdrawalOperationService actualizeAccountsDuringWithdrawalOperationService, 
            IActualizeOrdersDuringWithdrawalOperationService actualizeOrdersDuringWithdrawalOperationService,
            IActualizeDealsDuringWithdrawalOperationService actualizeDealsDuringWithdrawalOperationService,
            IAccountWithdrawalStartAggregateService withdrawalStartAggregateService,
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
            _actualizeAccountsDuringWithdrawalOperationService = actualizeAccountsDuringWithdrawalOperationService;
            _actualizeOrdersDuringWithdrawalOperationService = actualizeOrdersDuringWithdrawalOperationService;
            _actualizeDealsDuringWithdrawalOperationService = actualizeDealsDuringWithdrawalOperationService;
            _withdrawalStartAggregateService = withdrawalStartAggregateService;
            _withdrawalChangeStatusAggregateService = withdrawalChangeStatusAggregateService;
            _checkOperationPeriodService = checkOperationPeriodService;
            _aggregateServiceIsolator = aggregateServiceIsolator;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _createLockDetailsDuringWithdrawalOperationService = createLockDetailsDuringWithdrawalOperationService;
        }

        public WithdrawalProcessingResult Withdraw(long organizationUnitId, TimePeriod period)
        {
            _useCaseTuner.AlterDuration<WithdrawalOperationService>();

            WithdrawalInfo acquiredWithdrawal = null;

            try
            {
                string report;
                if (!TryAcquireTargetWithdrawal(organizationUnitId, period, out acquiredWithdrawal, out report))
                {
                    var msg = string.Format(
                                            "Can't acquire withdrawal for organization unit id {0} by period {1}. Error: {2}",
                                            organizationUnitId,
                                            period,
                                            report);

                    _logger.Error(msg);
                    return WithdrawalProcessingResult.Error(msg);
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    if (!LockSuccessfullyAcquired(acquiredWithdrawal))
                    {
                        var msg =
                            string.Format(
                                          "Acquired withdrawal with id {0} for organization unit with id {1} by period {2} has processing status violations. Possible reason for errors - concurrent withdrawal\reverting process and invalid withdrawal status processing",
                                          acquiredWithdrawal.Id,
                                          acquiredWithdrawal.OrganizationUnitId,
                                          period);

                        _logger.Error(msg);

                        transaction.Complete();
                        return WithdrawalProcessingResult.Error(msg);
                    }

                    var result = ExecuteWithdrawalProcessing(acquiredWithdrawal, organizationUnitId, period);

                    transaction.Complete();
                    return result;
                }
            }
            catch (WithdrawalException ex)
            {
                var msg = string.Format("Withdrawing aborted. An error occured. Organization unit id {0}. Period: {1}. Error: {2}",
                                        organizationUnitId,
                                        period,
                                        ex.Message);
                _logger.Error(ex, msg);

                return Abort(acquiredWithdrawal, msg);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Withdrawing aborted. Unexpected exception was caught. Organization unit id {0}. Period: {1}",
                                        organizationUnitId,
                                        period);
                _logger.Error(ex, msg);

                return Abort(acquiredWithdrawal, msg);
            }
        }

        private WithdrawalProcessingResult Abort(WithdrawalInfo acquiredWithdrawal, string msg)
        {
            if (acquiredWithdrawal != null)
            {
                _aggregateServiceIsolator.TransactedExecute<IAccountWithdrawalChangeStatusAggregateService>(TransactionScopeOption.RequiresNew,
                                                                                                            service =>
                                                                                                            service.Finish(acquiredWithdrawal, WithdrawalStatus.Error, msg));
            }

            return WithdrawalProcessingResult.Error(msg);
        }

        private static bool CanExecuteWithdrawing(WithdrawalInfo withdrawal, out string report)
        {
            report = null;

            if (withdrawal == null || withdrawal.Status == WithdrawalStatus.Error || withdrawal.Status == WithdrawalStatus.Reverted)
            {
                return true;
            }

            report = "Forbidden previous withdrawal status " + withdrawal.Status;
            return false;
        }

        private bool LockSuccessfullyAcquired(WithdrawalInfo acquiredWithdrawal)
        {
            var lockedWithdrawal =
                    _accountReadModel.GetLastWithdrawal(
                                            acquiredWithdrawal.OrganizationUnitId,
                                            new TimePeriod(acquiredWithdrawal.PeriodStartDate, acquiredWithdrawal.PeriodEndDate));
            return lockedWithdrawal != null
                    && lockedWithdrawal.Id == acquiredWithdrawal.Id
                    && lockedWithdrawal.Status == WithdrawalStatus.Withdrawing
                    && acquiredWithdrawal.SameVersionAs(lockedWithdrawal);
        }

        private WithdrawalProcessingResult ExecuteWithdrawalProcessing(WithdrawalInfo acquiredWithdrawal, long organizationUnitId, TimePeriod period)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<WithdrawalIdentity>())
            {
                _logger.InfoFormat("Withdrawing. Organization unit {0}. {1}. Starting lock details creation process",
                                     organizationUnitId,
                                     period);
                _createLockDetailsDuringWithdrawalOperationService.CreateLockDetails(organizationUnitId, period);

                var withdrawalInfos = _accountReadModel.GetInfoForWithdrawal(organizationUnitId, period);

                _logger.InfoFormat(
                    "Withdrawing. Organization unit {0}. {1}. Starting accounts actualization process. Target withdrawal infos count: {2}",
                    organizationUnitId,
                    period,
                    withdrawalInfos.Length);
                _actualizeAccountsDuringWithdrawalOperationService.Actualize(
                    period,
                    withdrawalInfos.Select(i => new AccountStateForWithdrawalDto
                        {
                            Lock = i.Lock,
                            Details = i.LockDetails,
                            LockBalance = i.CalculatedLockBalance,
                            Account = i.Account,
                            AccountBalanceBeforeWithdrawal = i.AccountBalanceBeforeWithdrawal,
                            OrderNumber = i.Order.Number
                        }));

                _logger.InfoFormat(
                    "Withdrawing. Organization unit {0}. {1}. Starting orders actualization process",
                    organizationUnitId,
                    period);
                _actualizeOrdersDuringWithdrawalOperationService.Actualize(
                    organizationUnitId,
                    withdrawalInfos.GroupBy(dto => dto.Order.Id, (l, dtos) => dtos.Single()).Select(dto => new ActualizeOrdersDto
                        {
                            Order = dto.Order,
                            AmountAlreadyWithdrawn = dto.AmountAlreadyWithdrawnAfterWithdrawal,
                            AmountToWithdrawNext = dto.AmountToWithdrawNextAfterWithdrawal
                        }));

                _logger.InfoFormat(
                    "Withdrawing. Organization unit {0}. {1}. Starting deals actualization process",
                    organizationUnitId,
                    period);
                _actualizeDealsDuringWithdrawalOperationService.Actualize(
                    withdrawalInfos
                        .Where(dto => dto.Order.DealId.HasValue)
                        .Select(dto => dto.Order.DealId.Value)
                        .Distinct());

                _withdrawalChangeStatusAggregateService.Finish(acquiredWithdrawal, WithdrawalStatus.Success, null);
                _logger.InfoFormat(
                    "Withdrawing process successfully finished. Organization unit {0}. {1}.",
                    organizationUnitId,
                    period);

                scope.Complete();
            }

            return WithdrawalProcessingResult.Succeeded;
        }

        private bool TryAcquireTargetWithdrawal(
            long organizationUnitId,
            TimePeriod period,
            out WithdrawalInfo acquiredWithdrawal,
            out string report)
        {
            acquiredWithdrawal = null;

            _logger.InfoFormat("Starting withdrawal process for organization unit with id {0} and time period {1}", organizationUnitId, period);

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, _userContext.Identity.Code))
            {
                report = "User doesn't have sufficient privileges for managing withdrawal";
                return false;
            }

            if (!_checkOperationPeriodService.IsOperationPeriodValid(period, out report))
            {
                return false;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var lastWithdrawal = _accountReadModel.GetLastWithdrawal(organizationUnitId, period);
                if (!CanExecuteWithdrawing(lastWithdrawal, out report))
                {
                    report = 
                        string.Format(
                            "Can't start withdrawal process for organization unit with id {0} and time period {1}. {2}",
                            organizationUnitId,
                            period,
                            report);

                    return false;
                }

                if (!_accountReadModel.HasActiveLocksForSourceOrganizationUnitByPeriod(organizationUnitId, period))
                {
                    report = 
                        string.Format(
                            "Can't start withdrawal process  for organization unit with id {0} and time period {1}. Active locks for orders not found, because final release have to be done before withdrawal",
                            organizationUnitId,
                            period);
                    return false;
                }

                acquiredWithdrawal = _withdrawalStartAggregateService.Start(organizationUnitId, period);
                transaction.Complete();
            }
            
            _logger.InfoFormat(
                    "Withdrawal process for organization unit {0} and period {1} is granted. Acquired withdrawal entry id {2}",
                    organizationUnitId,
                    period,
                    acquiredWithdrawal.Id);

            return true;
        }
    }
}
