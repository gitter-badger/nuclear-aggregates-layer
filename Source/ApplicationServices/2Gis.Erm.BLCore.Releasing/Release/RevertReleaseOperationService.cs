using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class RevertReleaseOperationService : IRevertReleaseOperationService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly ICheckOperationPeriodService _operationPeriodChecker;
        private readonly IAccountBulkDeleteLocksAggregateService _accountBulkDeleteLocksAggregateService;
        private readonly IAccountBulkReopenLimitsAggregateService _accountBulkReopenLimitsAggregateService;
        private readonly IReleaseChangeStatusAggregateService _changeReleaseStatusAggregateService;
        private readonly IAggregateServiceIsolator _aggregateServiceIsolator;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public RevertReleaseOperationService(IAccountReadModel accountReadModel,
                                             IReleaseReadModel releaseReadModel,
                                             ICheckOperationPeriodService operationPeriodChecker,
                                             IAccountBulkDeleteLocksAggregateService accountBulkDeleteLocksAggregateService,
                                             IAccountBulkReopenLimitsAggregateService accountBulkReopenLimitsAggregateService,
                                             IReleaseChangeStatusAggregateService changeReleaseStatusAggregateService,
                                             IAggregateServiceIsolator aggregateServiceIsolator,
                                             IUseCaseTuner useCaseTuner,
                                             IOperationScopeFactory scopeFactory,
                                             ICommonLog logger)
        {
            _accountReadModel = accountReadModel;
            _releaseReadModel = releaseReadModel;
            _operationPeriodChecker = operationPeriodChecker;
            _accountBulkDeleteLocksAggregateService = accountBulkDeleteLocksAggregateService;
            _accountBulkReopenLimitsAggregateService = accountBulkReopenLimitsAggregateService;
            _changeReleaseStatusAggregateService = changeReleaseStatusAggregateService;
            _aggregateServiceIsolator = aggregateServiceIsolator;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Revert(long organizationUnitId, TimePeriod period, string comment)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<RevertReleaseIdentity>())
            {
                _logger.InfoFormatEx("Attempting to revert release for organization unit with id {0} and by period {1}", organizationUnitId, period);
                var releaseInfo = ResolveRelease(organizationUnitId, period);
                if (_accountReadModel.HasInactiveLocksForDestinationOrganizationUnit(organizationUnitId, period))
                {
                    var msg = string.Format(BLResources.InactiveLocksExistsForPeriodAndOrganizatonUnit,
                                            period.Start,
                                            period.End,
                                            releaseInfo.OrganizationUnitName);
                    _logger.ErrorEx(msg);
                    throw new NotificationException(msg);
                }

                var previousStatus = (ReleaseStatus)releaseInfo.Release.Status;

                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                {
                    _changeReleaseStatusAggregateService.Reverting(releaseInfo.Release, comment);
                    transaction.Complete();
                }

                try
                {
                    _useCaseTuner.AlterDuration<RevertReleaseOperationService>();

                    _logger.InfoFormatEx("Reopening limits for organization units {0} and period {1}", releaseInfo.OrganizationUnitName, period);
                    var reopenLimits = _accountReadModel.GetClosedLimits(organizationUnitId, period);
                    _accountBulkReopenLimitsAggregateService.Reopen(reopenLimits);
                    _logger.InfoFormatEx("Limits reopened for organization units {0} and period {1}", releaseInfo.OrganizationUnitName, period);

                    _logger.InfoFormatEx("Deleting locks for organization units {0} and period {1}", releaseInfo.OrganizationUnitName, period);
                    var deletedLockInfos = _accountReadModel.GetActiveLocksForDestinationOrganizationUnitByPeriod(organizationUnitId, period);
                    _accountBulkDeleteLocksAggregateService.Delete(deletedLockInfos);
                    _logger.InfoFormatEx("Locks deleted for organization units {0} and period {1}", releaseInfo.OrganizationUnitName, period);

                    _changeReleaseStatusAggregateService.Reverted(releaseInfo.Release);
                    _logger.InfoFormatEx("Reverting release for organization unit with id {0} and by period {1} finished successfully",
                                         organizationUnitId,
                                         period);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Reverting release for organization unit with id {0} and by period {1} failed. Setting previous release state: {2}",
                                          organizationUnitId,
                                          period,
                                          previousStatus);

                    _aggregateServiceIsolator.TransactedExecute<IReleaseChangeStatusAggregateService>(
                        TransactionScopeOption.RequiresNew,
                        service => service.SetPreviousStatus(releaseInfo.Release, previousStatus, comment));
                    
                    _logger.ErrorFormatEx("Reverting release for organization unit with id {0} and by period {1} failed. Setting previous release state",
                                         organizationUnitId,
                                         period);

                    throw new ReleaseRevertingFailedException(ex.Message, ex);
                }

                scope.Complete();
            }
        }

        private ReleaseInfoDto ResolveRelease(long organizationUnitId, TimePeriod period)
        {
            string report;
            if (!_operationPeriodChecker.IsOperationPeriodValid(period, out report))
            {
                _logger.ErrorEx(report);
                throw new NotificationException(report);
            }

            var release = _releaseReadModel.GetLastRelease(organizationUnitId, period);
            if (!CanBeReverted(release, out report))
            {
                _logger.ErrorEx(report);
                throw new NotificationException(report);
            }

            return new ReleaseInfoDto
                {
                    Release = release,
                    OrganizationUnitName = _releaseReadModel.GetOrganizationUnitName(release.OrganizationUnitId),
                    Period = new TimePeriod(release.PeriodStartDate, release.PeriodEndDate)
                };
        }

        private static bool CanBeReverted(ReleaseInfo release, out string report)
        {
            report = null;

            if (release == null)
            {
                report = BLResources.CannotRevertReleaseBecausePreviousReleaseNotFound;
                return false;
            }

            bool canBeReverted = false;
            switch ((ReleaseStatus)release.Status)
            {
                case ReleaseStatus.InProgressInternalProcessingStarted:
                {
                    report = BLResources.CannotRevertReleaseBecausePreviousReleaseIsRunning;
                    break;
                }
                case ReleaseStatus.InProgressWaitingExternalProcessing:
                {
                    report = BLResources.CannotRevertReleaseBecausePreviousReleaseIsRunning;
                    break;
                }
                case ReleaseStatus.Reverting:
                {
                    report = BLResources.CannotRevertReleaseBecausePreviousReleaseIsRunning;
                    break;
                }
                case ReleaseStatus.Success:
                {
                    if (release.IsBeta)
                    {
                        report = BLResources.CannotRevertReleaseBecausePreviousReleaseWasTechnical;
                        break;
                    }

                    canBeReverted = true;
                    break;
                }
                case ReleaseStatus.Error:
                {
                    report = BLResources.CannotRevertReleaseBecausePreviousReleaseEndedWithErrors;
                    break;
                }
                case ReleaseStatus.Reverted:
                {
                    report = BLResources.AttemptToRepeatRevertReleaseOperation;
                    break;
                }
            }

            return canBeReverted;
        }
    }
}