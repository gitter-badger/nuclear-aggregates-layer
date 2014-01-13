﻿using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class StartReleaseOperationService : IStartReleaseOperationService
    {
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IReleaseStartAggregateService _releaseStartAggregateService;
        private readonly IReleaseChangeStatusAggregateService _releaseChangeStatusAggregateService;
        private readonly IReleaseAttachProcessingMessagesAggregateService _releaseAttachProcessingMessagesAggregateService;
        private readonly IValidateOrdersForReleaseOperationService _validateOrdersForReleaseOperationService;
        private readonly IEnsureOrdersForReleaseCompletelyExportedOperationService _ensureOrdersForReleaseCompletelyExportedOperationService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ICommonLog _logger;

        public StartReleaseOperationService(
            IReleaseReadModel releaseReadModel,
            IReleaseStartAggregateService releaseStartAggregateService,
            IReleaseChangeStatusAggregateService releaseChangeStatusAggregateService,
            IReleaseAttachProcessingMessagesAggregateService releaseAttachProcessingMessagesAggregateService,
            IValidateOrdersForReleaseOperationService validateOrdersForReleaseOperationService,
            IEnsureOrdersForReleaseCompletelyExportedOperationService ensureOrdersForReleaseCompletelyExportedOperationService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            IUseCaseTuner useCaseTuner,
            ICommonLog logger)
        {
            _releaseReadModel = releaseReadModel;
            _releaseStartAggregateService = releaseStartAggregateService;
            _releaseChangeStatusAggregateService = releaseChangeStatusAggregateService;
            _releaseAttachProcessingMessagesAggregateService = releaseAttachProcessingMessagesAggregateService;
            _validateOrdersForReleaseOperationService = validateOrdersForReleaseOperationService;
            _ensureOrdersForReleaseCompletelyExportedOperationService = ensureOrdersForReleaseCompletelyExportedOperationService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _useCaseTuner = useCaseTuner;
            _logger = logger;
        }

        public ReleaseStartingResult Start(int organizationUnitDgppId, TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors)
        {
            _useCaseTuner.AlterDuration<StartReleaseOperationService>();

            ReleaseInfo acquiredRelease = null;

            try
            {
                string report;
                if (!TryAcquireTargetRelease(organizationUnitDgppId, period, isBeta, canIgnoreBlockingErrors, out acquiredRelease, out report))
                {
                    _logger.ErrorFormatEx(
                        "Can't acquire release for organization unit with stable (DGPP) id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}. Error: {4}",
                                     organizationUnitDgppId,
                                     period,
                                     isBeta,
                                     canIgnoreBlockingErrors,
                                     report);

                    return new ReleaseStartingResult { Succeed = false, ProcessingMessages = new ReleaseProcessingMessage[0] };
                }

                return ExecuteInternalErmReleaseProcessing(acquiredRelease, organizationUnitDgppId, canIgnoreBlockingErrors);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Releasing aborted. Unexpected exception was caught. Release details: for organization unit with stable (DGPP) id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}",
                                                 organizationUnitDgppId,
                                                 period,
                                                 isBeta,
                                                 canIgnoreBlockingErrors);
                _logger.ErrorEx(ex, msg);

                if (acquiredRelease != null)
                {
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                    {
                        _releaseChangeStatusAggregateService.ChangeStatus(acquiredRelease, ReleaseStatus.Error, msg);
                        transaction.Complete();
                    }
                }

                throw;
            }
        }

        private bool TryAcquireTargetRelease(
            int organizationUnitDgppId, 
            TimePeriod period, 
            bool isBeta, 
            bool canIgnoreBlockingErrors,
            out ReleaseInfo acquiredRelease, 
            out string report)
        {
            acquiredRelease = null;

            // TODO {all, 04.11.2013}: проверить атуентифицируется ли экспорт при управлении сборкой в ERM и есть ли у него нужные права, если есть, то активировать проверку
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, _userContext.Identity.Code))
            {
                report = string.Empty;
                return true;
                //report = "User doesn't have sufficient privileges for starting release";
                //return false;
            }

            using (var scope = _scopeFactory.CreateNonCoupled<StartReleaseIdentity>())
            {
                _logger.InfoFormatEx(
                                     "Starting releasing for organization unit with stable (DGPP) id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}",
                                     organizationUnitDgppId,
                                     period,
                                     isBeta,
                                     canIgnoreBlockingErrors);

                var organizationUnitId = _releaseReadModel.GetOrganizationUnitId(organizationUnitDgppId);
                if (organizationUnitId == 0)
                {
                    report =
                        string.Format(
                                      "Can't continue release. Organization unit with stable (DGPP) id {0} not found. Release detail: {1} is beta {2}. Can ignore blocking errors: {3}",
                                      organizationUnitDgppId,
                                      period,
                                      isBeta,
                                      canIgnoreBlockingErrors);
                    _logger.ErrorEx(report);
                    return false;
                }

                if (!IsValidReleaseStartArgs(period, isBeta, canIgnoreBlockingErrors, out report))
                {
                    report =
                        string.Format(
                                      "Can't start releasing for organization unit with id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}. Error description: {4}",
                                      organizationUnitId,
                                      period,
                                      isBeta,
                                      canIgnoreBlockingErrors,
                                      report);

                    _logger.ErrorEx(report);
                    return false;
                }

                var previousRelease = _releaseReadModel.GetLastRelease(organizationUnitId, period);
                bool usingPreviouslyNotFinishedReleasing;
                if (!CanStartReleasing(previousRelease, out usingPreviouslyNotFinishedReleasing, out report))
                {
                    report =
                        string.Format(
                                      "Can't start releasing for organization unit with id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}. Error description: {4}",
                                      organizationUnitId,
                                      period,
                                      isBeta,
                                      canIgnoreBlockingErrors,
                                      report);

                    _logger.ErrorEx(report);

                    return false;
                }

                if (!usingPreviouslyNotFinishedReleasing)
                {
                    _logger.InfoFormatEx("Starting release for for organization unit with id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}",
                                          organizationUnitId,
                                          period,
                                          isBeta,
                                          canIgnoreBlockingErrors);

                    acquiredRelease = _releaseStartAggregateService.Start(organizationUnitId,
                                                        period,
                                                        isBeta,
                                                        ReleaseStatus.InProgressInternalProcessingStarted);
                }
                else
                {
                    var msg =
                        string.Format(
                                      "Using previously started release with id {0} for organization unit with id {1} by period {2}, that was not finished properly. Probably errors was detected on the external releasing side (Export and etc)",
                                      previousRelease.Id,
                                      organizationUnitId,
                                      period);

                    _logger.InfoEx(msg);
                    
                    _releaseChangeStatusAggregateService.ChangeStatus(previousRelease, ReleaseStatus.InProgressInternalProcessingStarted, msg);

                    acquiredRelease = previousRelease;
                }

                scope.Complete();
            }

            return true;
        }

        private ReleaseStartingResult ExecuteInternalErmReleaseProcessing(ReleaseInfo acquiredRelease, int organizationUnitDgppId, bool canIgnoreBlockingErrors)
        {
            var releasingResult = new ReleaseStartingResult
                {
                    Succeed = false,
                    ReleaseId = acquiredRelease.Id,
                    ProcessingMessages = new ReleaseProcessingMessage[0]
                };

            var releasingPeriod = new TimePeriod(acquiredRelease.PeriodStartDate, acquiredRelease.PeriodEndDate);
            
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                // TODO {i.maslennikov, 01.11.2013}: подумать нет ли необходимости сверять timestamp сохраненного acquiredRelease и полученного через lastrelease
                // смысл такой - нужно убедиться что после того как создали новую запись о сборке, переоткрыли старую, из конкурирующей транзакции не захватили ту же пессимистичную блокировку (ту же запись о сборке)
                var targetRelease = _releaseReadModel.GetLastRelease(acquiredRelease.OrganizationUnitId, releasingPeriod);
                if (targetRelease == null 
                    || targetRelease.Id != acquiredRelease.Id 
                    || (ReleaseStatus)targetRelease.Status != ReleaseStatus.InProgressInternalProcessingStarted)
                {
                    var msg = string.Format("Acquired release with id {0} for organization unit with id {1} by period {2} has processing status violations. Possible reason for errors - concurrent release\reverting process and invalid release status processing",
                                         acquiredRelease.Id,
                                         acquiredRelease.OrganizationUnitId,
                                         releasingPeriod);

                    _logger.ErrorEx(msg);
                    releasingResult.ProcessingMessages = new[] { new ReleaseProcessingMessage { IsBlocking = true, Message = msg } };

                    transaction.Complete();
                    return releasingResult;
                }

                var validationMessages = _validateOrdersForReleaseOperationService.Validate(targetRelease.OrganizationUnitId, releasingPeriod, targetRelease.IsBeta).ToArray();
                releasingResult.ProcessingMessages = validationMessages;

                bool hasBlockingErrors = validationMessages.Any(item => item.IsBlocking);
                _logger.InfoFormatEx(
                    "Release with id {0} for organization unit with id {1} by period {2}. After orders validation has blocking errors: {3} and can ingnore blocking errors: {4}",
                    targetRelease.Id,
                    targetRelease.OrganizationUnitId,
                    releasingPeriod,
                    hasBlockingErrors,
                    canIgnoreBlockingErrors);

                if (hasBlockingErrors && !canIgnoreBlockingErrors)
                {
                    var msg = string.Format(
                                "Aborting release with id {0} for organization unit with id {1} by period {2}. Has blocking errors that can't be ignored.",
                                targetRelease.Id,
                                targetRelease.OrganizationUnitId,
                                releasingPeriod);

                    _logger.ErrorEx(msg);
                    releasingResult.ProcessingMessages = AddNewBlockingMessage(releasingResult.ProcessingMessages, msg);

                    _releaseAttachProcessingMessagesAggregateService.SaveInternalMessages(targetRelease, validationMessages);
                    _releaseChangeStatusAggregateService.ChangeStatus(targetRelease, ReleaseStatus.Error, msg);

                    transaction.Complete();
                    return releasingResult;
                }

                if (!_ensureOrdersForReleaseCompletelyExportedOperationService.IsExported(targetRelease.Id,
                                                                                 targetRelease.OrganizationUnitId,
                                                                                 organizationUnitDgppId,
                                                                                 releasingPeriod,
                                                                                 targetRelease.IsBeta))
                {
                    var msg = string.Format("Releasing aborted. Ensure process detected that not all required orders for release are properly exported. Release details: id {0} for organization unit with id {1} by period {2}",
                                            targetRelease.Id,
                                            targetRelease.OrganizationUnitId,
                                            releasingPeriod);
                    _logger.ErrorEx(msg);
                    releasingResult.ProcessingMessages = AddNewBlockingMessage(releasingResult.ProcessingMessages, msg);

                    _releaseChangeStatusAggregateService.ChangeStatus(targetRelease, ReleaseStatus.Error, msg);

                    transaction.Complete();
                    return releasingResult;
                }

                releasingResult.Succeed = true;
                _releaseChangeStatusAggregateService.ChangeStatus(targetRelease, ReleaseStatus.InProgressWaitingExternalProcessing, string.Empty);

                _logger.InfoFormatEx("Release with id {0} successfully started for organization unit with id {1} by period {2} is beta {3}. Waiting for external release processing",
                                   targetRelease.Id,
                                   targetRelease.OrganizationUnitId,
                                   releasingPeriod,
                                   targetRelease.IsBeta);

                transaction.Complete();
            }

            return releasingResult;
        }

        private bool IsValidReleaseStartArgs(TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors, out string report)
        {
            report = null;

            if (period.Start.Day != 1)
            {
                report = "Period have to start at first day of month";
                return false;
            }

            if (period.Start != period.End.GetFirstDateOfMonth())
            {
                report = "Period start and period end days must be in the same month";
                return false;
            }

            var daysInMonth = DateTime.DaysInMonth(period.End.Year, period.End.Month);
            if (!(period.End.Day == daysInMonth && period.End.Hour == 23 && period.End.Minute == 59 && period.End.Second == 59))
            {
                report = "Period end have to be " + daysInMonth + " day of month with time value 23:59:59";
                return false;
            }

            if (!isBeta && canIgnoreBlockingErrors)
            {
                report = "Final release can't use ignore blocking errors mode";
                return false;
            }

            return true;
        }

        private bool CanStartReleasing(ReleaseInfo previousRelease, out bool usingPreviouslyNotFinishedReleasing, out string report)
        {
            report = string.Empty;
            usingPreviouslyNotFinishedReleasing = false;

            if (previousRelease == null)
            {
                return true;
            }

            var previousReleasePeriod = new TimePeriod(previousRelease.PeriodStartDate, previousRelease.PeriodEndDate);
            var previousReleaseStatus = (ReleaseStatus)previousRelease.Status;
            switch (previousReleaseStatus)
            {
                case ReleaseStatus.Success:
                {
                    if (previousRelease.IsBeta)
                    {
                        return true;
                    }
                    
                    report = string.Format("Previous release with id {0} for organization unit with id {1} by period {2} is final and success status. Can't start new release without reverting previous final and successful release",
                                               previousRelease.Id,
                                               previousRelease.OrganizationUnitId,
                                               previousReleasePeriod);

                    return false;
                }
                case ReleaseStatus.Error:
                case ReleaseStatus.Reverted:
                {
                    return true;
                }
                case ReleaseStatus.InProgressWaitingExternalProcessing:
                {
                    usingPreviouslyNotFinishedReleasing = true;
                    return true;
                }
            }

            report = string.Format("Previous release with id {0} for organization unit with id {1} by period {2} has status {3}, so new releasing can't be started",
                                               previousRelease.Id,
                                               previousRelease.OrganizationUnitId,
                                               previousReleasePeriod,
                                               previousReleaseStatus);

            return false;
        }

        private ReleaseProcessingMessage[] AddNewBlockingMessage(ReleaseProcessingMessage[] processingMessages, string newMessage)
        {
            var extendedMessages = new ReleaseProcessingMessage[processingMessages.Length + 1];
            extendedMessages[0] = new ReleaseProcessingMessage { IsBlocking = true, Message = newMessage };

            processingMessages.CopyTo(extendedMessages, 1);

            return extendedMessages;
        }
    }
}
