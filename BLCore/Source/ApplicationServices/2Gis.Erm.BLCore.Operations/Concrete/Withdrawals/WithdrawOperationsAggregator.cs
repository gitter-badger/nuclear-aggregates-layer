using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.UseCases;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class WithdrawOperationsAggregator : IWithdrawOperationsAggregator
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IWithdrawOperationService _withdrawOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IMonthPeriodValidationService _checkPeriodService;
        private readonly IUserContext _userContext;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ITracer _tracer;
        private readonly IOperationService _operationService;
        private readonly IGetWithdrawalErrorsCsvReportOperationService _getWithdrawalErrorsCsvReportOperationService;

        public WithdrawOperationsAggregator(IAccountReadModel accountReadModel,
                                            IWithdrawOperationService withdrawOperationService,
                                            ISecurityServiceFunctionalAccess functionalAccessService,
                                            IMonthPeriodValidationService checkPeriodService,
                                            IUserContext userContext,
                                            IUseCaseTuner useCaseTuner,
                                            ITracer tracer,
                                            IOperationService operationService,
                                            IGetWithdrawalErrorsCsvReportOperationService getWithdrawalErrorsCsvReportOperationService)
        {
            _accountReadModel = accountReadModel;
            _withdrawOperationService = withdrawOperationService;
            _functionalAccessService = functionalAccessService;
            _checkPeriodService = checkPeriodService;
            _userContext = userContext;
            _useCaseTuner = useCaseTuner;
            _tracer = tracer;
            _operationService = operationService;
            _getWithdrawalErrorsCsvReportOperationService = getWithdrawalErrorsCsvReportOperationService;
        }

        public BulkWithdrawResult Withdraw(TimePeriod period, AccountingMethod accountingMethod, out Guid businessOperationId)
        {
            _useCaseTuner.AlterDuration<WithdrawOperationsAggregator>();
            businessOperationId = Guid.Empty;

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, _userContext.Identity.Code))
            {
                throw new OperationAccessDeniedException("User doesn't have sufficient privileges for managing withdrawal");
            }

            string report;
            if (!_checkPeriodService.IsValid(period, out report))
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

            var organizationUnits = _accountReadModel.GetOrganizationUnitsToProccessWithdrawals(period.Start, period.End, accountingMethod);
            if (!organizationUnits.Any())
            {
                return BulkWithdrawResult.NoSuitableDataFound;
            }

            var processingResultsByOrganizationUnit = new Dictionary<long, WithdrawalProcessingResult>();
            foreach (var organizationUnit in organizationUnits)
            {
                try
                {
                    processingResultsByOrganizationUnit.Add(organizationUnit, _withdrawOperationService.Withdraw(organizationUnit, period, accountingMethod));
                }
                catch (Exception ex)
                {
                    processingResultsByOrganizationUnit.Add(organizationUnit, WithdrawalProcessingResult.Errors(ex.ToString()));
                    _tracer.ErrorFormat(ex, "Не удалось провести списание по отделению организации {0}", organizationUnit);
                }
            }

            var allWithwrawalsSucceded = processingResultsByOrganizationUnit.All(x => x.Value.Succeded);
            businessOperationId = Guid.NewGuid();

            var operation = new Operation
                                {
                                    Guid = businessOperationId,
                                    StartTime = DateTime.UtcNow,
                                    FinishTime = DateTime.UtcNow,
                                    OwnerCode = _userContext.Identity.Code,
                                    Status = allWithwrawalsSucceded ? OperationStatus.Success : OperationStatus.Error,
                                    Type = BusinessOperation.Withdrawal,
                                };

            var csvReport = allWithwrawalsSucceded
                                ? new WithdrawalErrorsReport()
                                : _getWithdrawalErrorsCsvReportOperationService.GetErrorsReport(processingResultsByOrganizationUnit.Where(x => !x.Value.Succeded)
                                                                                                                                   .ToDictionary(x => x.Key, y => y.Value),
                                                                                                period,
                                                                                                accountingMethod);

            // ВНИМАНИЕ! В текущей реализации есть вероятность не сохранить эту сущность → инфа об ошибках потеряется.
            // Однако, если в этом случае можно запустить списание еще раз, чтобы получить этот отчет. И так до победного.
            _operationService.CreateOperation(operation,
                                              csvReport.ReportContent,
                                              HttpUtility.UrlPathEncode(csvReport.ReportFileName),
                                              csvReport.ContentType);

            return allWithwrawalsSucceded ? BulkWithdrawResult.AllSucceeded : BulkWithdrawResult.ErrorsOccurred;
        }
    }
}
