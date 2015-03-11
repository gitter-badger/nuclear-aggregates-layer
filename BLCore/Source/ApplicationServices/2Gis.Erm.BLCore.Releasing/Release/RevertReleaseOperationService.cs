using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

using NuClear.Tracing.API;

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
        private readonly ITracer _tracer;

        public RevertReleaseOperationService(IAccountReadModel accountReadModel,
                                             IReleaseReadModel releaseReadModel,
                                             ICheckOperationPeriodService operationPeriodChecker,
                                             IAccountBulkDeleteLocksAggregateService accountBulkDeleteLocksAggregateService,
                                             IAccountBulkReopenLimitsAggregateService accountBulkReopenLimitsAggregateService,
                                             IReleaseChangeStatusAggregateService changeReleaseStatusAggregateService,
                                             IAggregateServiceIsolator aggregateServiceIsolator,
                                             IUseCaseTuner useCaseTuner,
                                             IOperationScopeFactory scopeFactory,
                                             ITracer tracer)
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
            _tracer = tracer;
        }

        public ReleaseRevertingResult Revert(long organizationUnitId, TimePeriod period, string comment)
        {
            _tracer.InfoFormat("Attempting to revert release for organization unit with id {0} and by period {1}", organizationUnitId, period);
            
            _useCaseTuner.AlterDuration<RevertReleaseOperationService>();

            ReleaseInfo acquiredRelease = null;

            try
            {
                string report;
                AcquiredReleaseDescriptor acquiredReleaseDescriptor;
                if (!TryAcquireTargetRelease(organizationUnitId,
                                            period,
                                            comment,
                                            out acquiredRelease,
                                            out acquiredReleaseDescriptor,
                                            out report))
                {
                    var msg = string.Format("Reverting release failed. " +
                                            "Can't acquire release for organization unit with id {0} by period {1}. Details: {2}",
                                            organizationUnitId,
                                            period,
                                            report);
                    _tracer.Error(msg);
                    return ReleaseRevertingResult.Error(msg);
                }

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    if (!LockSuccessfullyAcquired(acquiredRelease))
                    {
                        var msg = string.Format("Release reverting. Acquired release with id {0} for organization unit with id {1} by period {2} has processing status violations. " +
                                                "Possible reason for errors - concurrent release\reverting process and invalid release status processing",
                                                acquiredRelease.Id,
                                                acquiredRelease.OrganizationUnitId,
                                                period);
                        _tracer.Error(msg);
                        return ReleaseRevertingResult.Error(msg);
                    }

                    var result = ExecuteRevertReleaseProcessing(acquiredRelease, organizationUnitId, period, acquiredReleaseDescriptor);
                    
                    transaction.Complete();

                    return result;
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Reverting release for organization unit with id {0} and by period {1} failed.",
                                        organizationUnitId,
                                        period);
                _tracer.Error(ex, msg);

                if (acquiredRelease != null)
                {
                    _tracer.ErrorFormat("Reverting release with id {0} failed. Organization unit: {1}. {2}. Restoring release status to success value",
                                          acquiredRelease.Id,
                                          organizationUnitId,
                                          period);

                    _aggregateServiceIsolator.TransactedExecute<IReleaseChangeStatusAggregateService>(
                        TransactionScopeOption.RequiresNew,
                        service => service.SetPreviousStatus(acquiredRelease, ReleaseStatus.Success, "Restored status, after reverting attempt failed"));

                    _tracer.ErrorFormat("Reverting release with id {0} failed. Organization unit: {1}. {2}. Release status restored to success value",
                                          acquiredRelease.Id,
                                          organizationUnitId,
                                          period);
                }

                return ReleaseRevertingResult.Error(msg);
            }
        }

        private static bool CanBeReverted(ReleaseInfo lastFinalRelease, out string report)
        {
            report = null;

            if (lastFinalRelease == null)
            {
                report = BLResources.CannotRevertReleaseBecausePreviousReleaseNotFound;
                return false;
            }

            bool canBeReverted = false;
            switch (lastFinalRelease.Status)
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
                    if (lastFinalRelease.IsBeta)
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

        private bool LockSuccessfullyAcquired(ReleaseInfo acquiredRelease)
        {
            var lockedRelease = _releaseReadModel.GetLastFinalRelease(acquiredRelease.OrganizationUnitId,
                                                                      new TimePeriod(acquiredRelease.PeriodStartDate, acquiredRelease.PeriodEndDate));
            return lockedRelease != null &&
                   lockedRelease.Id == acquiredRelease.Id &&
                   lockedRelease.Status == ReleaseStatus.Reverting &&
                   acquiredRelease.SameVersionAs(lockedRelease);
        }

        private bool TryAcquireTargetRelease(long organizationUnitId,
            TimePeriod period,
            string comment,
            out ReleaseInfo acquiredRelease,
            out AcquiredReleaseDescriptor acquiredReleaseDescriptor,
            out string report)
        {
            acquiredRelease = null;
            acquiredReleaseDescriptor = new AcquiredReleaseDescriptor();

            if (!_operationPeriodChecker.IsOperationPeriodValid(period, out report))
            {
                _tracer.Error(report);
                return false;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var lastFinalRelease = _releaseReadModel.GetLastFinalRelease(organizationUnitId, period);
                if (!CanBeReverted(lastFinalRelease, out report))
                {
                    _tracer.Error(report);
                    return false;
                }

                acquiredReleaseDescriptor.OrganizationUnitName = _releaseReadModel.GetOrganizationUnitName(lastFinalRelease.OrganizationUnitId);
                if (_accountReadModel.HasInactiveLocksForDestinationOrganizationUnit(organizationUnitId, period))
                {
                    report = string.Format(BLResources.InactiveLocksExistsForPeriodAndOrganizatonUnit,
                                            period.Start,
                                            period.End,
                                            acquiredReleaseDescriptor.OrganizationUnitName);
                    _tracer.Error(report);
                    return false;
                }

                _changeReleaseStatusAggregateService.Reverting(lastFinalRelease, comment);
                acquiredRelease = lastFinalRelease;

                transaction.Complete();
            }

            _tracer.InfoFormat("Reverting release process for organization unit {0} and period {1} is granted. Acquired release entry id {2}",
                    organizationUnitId,
                    period,
                    acquiredRelease.Id);

            return true;
        }

        private ReleaseRevertingResult ExecuteRevertReleaseProcessing(ReleaseInfo acquiredRelease, long organizationUnitId, TimePeriod period, AcquiredReleaseDescriptor acquiredReleaseDescriptor)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<RevertReleaseIdentity>())
            {
                _tracer.InfoFormat("Reopening limits for organization units {0} and period {1}", acquiredReleaseDescriptor.OrganizationUnitName, period);
                var reopenLimits = _accountReadModel.GetClosedLimits(organizationUnitId, period);
                _accountBulkReopenLimitsAggregateService.Reopen(reopenLimits);
                _tracer.InfoFormat("Limits reopened for organization units {0} and period {1}", acquiredReleaseDescriptor.OrganizationUnitName, period);

                _tracer.InfoFormat("Deleting locks for organization units {0} and period {1}", acquiredReleaseDescriptor.OrganizationUnitName, period);
                var deletedLockInfos = _accountReadModel.GetActiveLocksForDestinationOrganizationUnitByPeriod(organizationUnitId, period);
                _accountBulkDeleteLocksAggregateService.Delete(deletedLockInfos);
                _tracer.InfoFormat("Locks deleted for organization units {0} and period {1}", acquiredReleaseDescriptor.OrganizationUnitName, period);

                _changeReleaseStatusAggregateService.Reverted(acquiredRelease);
                _tracer.InfoFormat("Reverting release for organization unit with id {0} and by period {1} finished successfully",
                                     organizationUnitId,
                                     period);
                scope.Complete();
            }

            return ReleaseRevertingResult.Succeeded;
        }

        private class AcquiredReleaseDescriptor
        {
            public string OrganizationUnitName { get; set; }
        }
    }
}