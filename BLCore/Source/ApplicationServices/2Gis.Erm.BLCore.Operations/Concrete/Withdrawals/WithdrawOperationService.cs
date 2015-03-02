using System;
using System.Collections.Generic;
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
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class WithdrawOperationService : IWithdrawOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IActualizeAccountsDuringWithdrawalOperationService _actualizeAccountsDuringWithdrawalOperationService;
        private readonly IActualizeOrdersDuringWithdrawalOperationService _actualizeOrdersDuringWithdrawalOperationService;
        private readonly IActualizeDealsDuringWithdrawalOperationService _actualizeDealsDuringWithdrawalOperationService;
        private readonly IAccountWithdrawalStartAggregateService _withdrawalStartAggregateService;
        private readonly IAccountWithdrawalChangeStatusAggregateService _withdrawalChangeStatusAggregateService;
        private readonly IAggregateServiceIsolator _aggregateServiceIsolator;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;
        private readonly ICreateLockDetailsDuringWithdrawalOperationService _createLockDetailsDuringWithdrawalOperationService;
        private readonly IWithdrawalOperationValidationRulesProvider _withdrawalOperationValidationRulesProvider;

        public WithdrawOperationService(
            IAccountReadModel accountReadModel,
            ICreateLockDetailsDuringWithdrawalOperationService createLockDetailsDuringWithdrawalOperationService,
            IActualizeAccountsDuringWithdrawalOperationService actualizeAccountsDuringWithdrawalOperationService, 
            IActualizeOrdersDuringWithdrawalOperationService actualizeOrdersDuringWithdrawalOperationService,
            IActualizeDealsDuringWithdrawalOperationService actualizeDealsDuringWithdrawalOperationService,
            IAccountWithdrawalStartAggregateService withdrawalStartAggregateService,
            IAccountWithdrawalChangeStatusAggregateService withdrawalChangeStatusAggregateService,
            IAggregateServiceIsolator aggregateServiceIsolator,
            IUseCaseTuner useCaseTuner,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger,
            IWithdrawalOperationValidationRulesProvider withdrawalOperationValidationRulesProvider)
        {
            _accountReadModel = accountReadModel;
            _actualizeAccountsDuringWithdrawalOperationService = actualizeAccountsDuringWithdrawalOperationService;
            _actualizeOrdersDuringWithdrawalOperationService = actualizeOrdersDuringWithdrawalOperationService;
            _actualizeDealsDuringWithdrawalOperationService = actualizeDealsDuringWithdrawalOperationService;
            _withdrawalStartAggregateService = withdrawalStartAggregateService;
            _withdrawalChangeStatusAggregateService = withdrawalChangeStatusAggregateService;
            _aggregateServiceIsolator = aggregateServiceIsolator;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _withdrawalOperationValidationRulesProvider = withdrawalOperationValidationRulesProvider;
            _createLockDetailsDuringWithdrawalOperationService = createLockDetailsDuringWithdrawalOperationService;
        }

        public WithdrawalProcessingResult Withdraw(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            _useCaseTuner.AlterDuration<WithdrawOperationService>();

            var withdrawOperationParametersDescription = GetOperationParametersDescription(organizationUnitId, period, accountingMethod);

            WithdrawalInfo acquiredWithdrawal = null;

            try
            {
                IEnumerable<string> report;
                if (!TryAcquireTargetWithdrawal(organizationUnitId, period, accountingMethod, out acquiredWithdrawal, out report))
                {
                    var msg = string.Format("Can't acquire withdrawal. {0} Errors: {1}",
                                            withdrawOperationParametersDescription,
                                            string.Join(Environment.NewLine, report));

                    _logger.Error(msg);
                    return WithdrawalProcessingResult.Errors(report.ToArray());
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    if (!LockSuccessfullyAcquired(acquiredWithdrawal))
                    {
                        var msg =
                            string.Format("Acquired withdrawal with id {0} has processing status violations. Possible reason for errors - concurrent withdrawal\reverting process and invalid withdrawal status processing. {1}",
                                          acquiredWithdrawal.Id,
                                          withdrawOperationParametersDescription);

                        _logger.Error(msg);

                        transaction.Complete();
                        return WithdrawalProcessingResult.Errors(msg);
                    }

                    var result = ExecuteWithdrawalProcessing(acquiredWithdrawal, organizationUnitId, period, accountingMethod);

                    transaction.Complete();
                    return result;
                }
            }
            catch (WithdrawalException ex)
            {
                var msg = string.Format("Withdrawing aborted. An error occured. {0} Error: {1}",
                                        withdrawOperationParametersDescription,
                                        ex.Message);
                _logger.Error(ex, msg);

                return Abort(acquiredWithdrawal, msg);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Withdrawing aborted. Unexpected exception was caught. {0}",
                                        withdrawOperationParametersDescription);
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

            return WithdrawalProcessingResult.Errors(msg);
        }

        private bool LockSuccessfullyAcquired(WithdrawalInfo acquiredWithdrawal)
        {
            var lockedWithdrawal =
                _accountReadModel.GetLastWithdrawal(acquiredWithdrawal.OrganizationUnitId,
                                                    new TimePeriod(acquiredWithdrawal.PeriodStartDate, acquiredWithdrawal.PeriodEndDate),
                                                    acquiredWithdrawal.AccountingMethod);
            return lockedWithdrawal != null
                    && lockedWithdrawal.Id == acquiredWithdrawal.Id
                    && lockedWithdrawal.Status == WithdrawalStatus.Withdrawing
                    && acquiredWithdrawal.SameVersionAs(lockedWithdrawal);
        }

        private WithdrawalProcessingResult ExecuteWithdrawalProcessing(WithdrawalInfo acquiredWithdrawal,
                                                                       long organizationUnitId,
                                                                       TimePeriod period,
                                                                       AccountingMethod accountingMethod)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<WithdrawIdentity>())
            {
                var withdrawOperationParametersDescription = GetOperationParametersDescription(organizationUnitId, period, accountingMethod);
                _logger.InfoFormat("Withdrawing. {0} Starting lock details creation process",
                                   withdrawOperationParametersDescription);

                if (accountingMethod == AccountingMethod.PlannedProvision)
                {
                    _createLockDetailsDuringWithdrawalOperationService.CreateLockDetails(organizationUnitId, period);
                }

                var withdrawalInfos = _accountReadModel.GetInfoForWithdrawal(organizationUnitId, period, accountingMethod);

                _logger.InfoFormat("Withdrawing. {0} Starting accounts actualization process. Target withdrawal infos count: {1}",
                                   withdrawOperationParametersDescription,
                                   withdrawalInfos.Length);
                _actualizeAccountsDuringWithdrawalOperationService.Actualize(period,
                                                                             withdrawalInfos.Select(i => new AccountStateForWithdrawalDto
                                                                                                             {
                                                                                                                 Lock = i.Lock,
                                                                                                                 Details = i.LockDetails,
                                                                                                                 LockBalance = i.CalculatedLockBalance,
                                                                                                                 Account = i.Account,
                                                                                                                 AccountBalanceBeforeWithdrawal = i.AccountBalanceBeforeWithdrawal,
                                                                                                                 OrderNumber = i.Order.Number
                                                                                                             }));

                _logger.InfoFormat("Withdrawing. {0} Starting orders actualization process",
                                   withdrawOperationParametersDescription);
                _actualizeOrdersDuringWithdrawalOperationService.Actualize(organizationUnitId,
                                                                           withdrawalInfos.GroupBy(dto => dto.Order.Id, (l, dtos) => dtos.Single())
                                                                                          .Select(dto => new ActualizeOrdersDto
                                                                                                             {
                                                                                                                 Order = dto.Order,
                                                                                                                 AmountAlreadyWithdrawn = dto.AmountAlreadyWithdrawnAfterWithdrawal,
                                                                                                                 AmountToWithdrawNext = dto.AmountToWithdrawNextAfterWithdrawal
                                                                                                             }));

                _logger.InfoFormat("Withdrawing. {0} Starting deals actualization process",
                                   withdrawOperationParametersDescription);
                _actualizeDealsDuringWithdrawalOperationService.Actualize(withdrawalInfos
                                                                              .Where(dto => dto.Order.DealId.HasValue)
                                                                              .Select(dto => dto.Order.DealId.Value)
                                                                              .Distinct());

                _withdrawalChangeStatusAggregateService.Finish(acquiredWithdrawal, WithdrawalStatus.Success, null);
                _logger.InfoFormat("Withdrawing process successfully finished. {0}",
                                   withdrawOperationParametersDescription);

                scope.Complete();
            }

            return WithdrawalProcessingResult.Succeeded;
        }

        private bool TryAcquireTargetWithdrawal(long organizationUnitId,
                                                TimePeriod period,
                                                AccountingMethod accountingMethod,
                                                out WithdrawalInfo acquiredWithdrawal,
                                                out IEnumerable<string> report)
        {
            acquiredWithdrawal = null;
            var withdrawOperationParametersDescription = GetOperationParametersDescription(organizationUnitId, period, accountingMethod);

            _logger.InfoFormat("Starting withdrawal process. {0}", withdrawOperationParametersDescription);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var validationRules = _withdrawalOperationValidationRulesProvider.GetValidationRules();

                foreach (var validationRule in validationRules)
                {
                    if (!validationRule.Validate(organizationUnitId, period, accountingMethod, out report))
                    {
                        transaction.Complete();
                        return false;
                    }
                }

                acquiredWithdrawal = _withdrawalStartAggregateService.Start(organizationUnitId, period, accountingMethod);
                transaction.Complete();
            }

            _logger.InfoFormat("Withdrawal process for organization is granted. {0} Acquired withdrawal entry id {1}",
                               withdrawOperationParametersDescription,
                               acquiredWithdrawal.Id);

            report = Enumerable.Empty<string>();
            return true;
        }

        private string GetOperationParametersDescription(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            return string.Format("Organization unit id: {0}. Period: {1}. Accounting method {2}.",
                                 organizationUnitId,
                                 period,
                                 accountingMethod);
        }
    }
}
