using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class WithdrawalsByAccountingMethodOperationService : IWithdrawalsByAccountingMethodOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IWithdrawalOperationService _withdrawalOperationService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ICheckOperationPeriodService _checkOperationPeriodService;
        private readonly IUserContext _userContext;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ICommonLog _commonLog;

        public WithdrawalsByAccountingMethodOperationService(IAccountReadModel accountReadModel,
                                                             IWithdrawalOperationService withdrawalOperationService,
                                                             IOperationScopeFactory operationScopeFactory,
                                                             ISecurityServiceFunctionalAccess functionalAccessService,
                                                             ICheckOperationPeriodService checkOperationPeriodService,
                                                             IUserContext userContext,
                                                             IUseCaseTuner useCaseTuner,
                                                             ICommonLog commonLog)
        {
            _accountReadModel = accountReadModel;
            _withdrawalOperationService = withdrawalOperationService;
            _operationScopeFactory = operationScopeFactory;
            _functionalAccessService = functionalAccessService;
            _checkOperationPeriodService = checkOperationPeriodService;
            _userContext = userContext;
            _useCaseTuner = useCaseTuner;
            _commonLog = commonLog;
        }

        public IDictionary<long, WithdrawalProcessingResult> Withdraw(TimePeriod period, AccountingMethod accountingMethod)
        {
            _useCaseTuner.AlterDuration<WithdrawalsByAccountingMethodOperationService>();

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException("User doesn't have sufficient privileges for managing withdrawal");
            }

            string report;
            if (!_checkOperationPeriodService.IsOperationPeriodValid(period, out report))
            {
                throw new InvalidPeriodException(report);
            }

            if (accountingMethod == AccountingMethod.PlannedProvision)
            {
                // TODO {all, 23.05.2014}: Проверка отключена - https://jira.2gis.ru/browse/ERM-4092
                //var actualCharges = _withdrawalReadModel.GetActualChargesByProject(period);

                //var projectsWithoutCharges = actualCharges.Where(x => x.Value == null).Select(x => x.Key).ToArray();
                //if (projectsWithoutCharges.Any())
                //{
                //    throw new MissingChargesForProjectException(
                //        string.Format("Can't create lock details before withdrawing. The following projects have no charges: {0}.",
                //                      string.Join(", ", projectsWithoutCharges)));
                //}
            }            

            using (var scope = _operationScopeFactory.CreateNonCoupled<WithdrawalsByAccountingMethodIdentity>())
            {
                var organizationUnits = _accountReadModel.GetOrganizationUnitsToProccessWithdrawals(period.Start, period.End, accountingMethod);
                var result = new Dictionary<long, WithdrawalProcessingResult>();
                foreach (var organizationUnit in organizationUnits)
                {
                    try
                    {
                        using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                        {
                            result.Add(organizationUnit, _withdrawalOperationService.Withdraw(organizationUnit, period, accountingMethod));
                            transactionScope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Add(organizationUnit, WithdrawalProcessingResult.Errors(ex.ToString()));
                        _commonLog.ErrorFormat(ex, "Не удалось провести списание по отделению организации {0}", organizationUnit);
                    }
                }

                scope.Complete();

                return result;
            }
        }
    }
}
