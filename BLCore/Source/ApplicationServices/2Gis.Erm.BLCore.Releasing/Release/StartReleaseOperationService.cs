using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

using NuClear.Aggregates;
using NuClear.Storage.UseCases;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class StartReleaseOperationService : IStartReleaseOperationService
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IEnumerable<IReleaseStartingOptionConditionSet> _releaseStartingOptionConditionSets;
        private readonly IReleaseStartAggregateService _releaseStartAggregateService;
        private readonly IReleaseChangeStatusAggregateService _releaseChangeStatusAggregateService;
        private readonly IReleaseAttachProcessingMessagesAggregateService _releaseAttachProcessingMessagesAggregateService;
        private readonly IValidateOrdersForReleaseOperationService _validateOrdersForReleaseOperationService;
        private readonly IEnsureOrdersForReleaseCompletelyExportedOperationService _ensureOrdersForReleaseCompletelyExportedOperationService;
        private readonly IAggregateServiceIsolator _aggregateServiceIsolator;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ITracer _tracer;

        public StartReleaseOperationService(IIntegrationSettings integrationSettings,
                                            IReleaseReadModel releaseReadModel,
                                            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
                                            IReleaseStartingOptionConditionSet[] releaseStartingOptionConditionSets,
                                            IReleaseStartAggregateService releaseStartAggregateService,
                                            IReleaseChangeStatusAggregateService releaseChangeStatusAggregateService,
                                            IReleaseAttachProcessingMessagesAggregateService releaseAttachProcessingMessagesAggregateService,
                                            IValidateOrdersForReleaseOperationService validateOrdersForReleaseOperationService,
                                            IEnsureOrdersForReleaseCompletelyExportedOperationService ensureOrdersForReleaseCompletelyExportedOperationService,
                                            IAggregateServiceIsolator aggregateServiceIsolator,
                                            ISecurityServiceFunctionalAccess functionalAccessService,
                                            IUserContext userContext,
                                            IOperationScopeFactory scopeFactory,
                                            IUseCaseTuner useCaseTuner,
                                            ITracer tracer)
        {
            _integrationSettings = integrationSettings;
            _releaseReadModel = releaseReadModel;
            _releaseStartingOptionConditionSets = releaseStartingOptionConditionSets;
            _releaseStartAggregateService = releaseStartAggregateService;
            _releaseChangeStatusAggregateService = releaseChangeStatusAggregateService;
            _releaseAttachProcessingMessagesAggregateService = releaseAttachProcessingMessagesAggregateService;
            _validateOrdersForReleaseOperationService = validateOrdersForReleaseOperationService;
            _ensureOrdersForReleaseCompletelyExportedOperationService = ensureOrdersForReleaseCompletelyExportedOperationService;
            _aggregateServiceIsolator = aggregateServiceIsolator;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _useCaseTuner = useCaseTuner;
            _tracer = tracer;
        }

        public ReleaseStartingResult Start(int organizationUnitDgppId, TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                throw new InvalidOperationException("Interaction with service bus is disabled in Erm config settings. Release cannot be started");
            }

            // Текущая реализация определения identity текущего пользователя установит в IUserContext.Identity учетку пула приложений IIS, если безопасность выключена для endpoint
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, _userContext.Identity.Code))
            {
                throw new SecurityAccessDeniedException("User doesn't have sufficient privileges for starting release");
            }

            _useCaseTuner.AlterDuration<StartReleaseOperationService>();

            ReleaseInfo acquiredRelease = null;

            try
            {
                long? previousReleaseId;
                string report;
                if (!TryAcquireTargetRelease(organizationUnitDgppId,
                                            period,
                                            isBeta,
                                            canIgnoreBlockingErrors,
                                            out acquiredRelease,
                                            out previousReleaseId,
                                            out report))
                {
                    _tracer.ErrorFormat("Releasing. Can't acquire release for organization unit with stable (DGPP) id {0} by period {1} is beta = {2}. " +
                                          "Can ignore blocking errors: {3}. Error: {4}",
                                          organizationUnitDgppId,
                                          period,
                                          isBeta,
                                          canIgnoreBlockingErrors,
                                          report);

                    return new ReleaseStartingResult
                        {
                            Succeed = false,
                            ReleaseId = previousReleaseId.HasValue ? previousReleaseId.Value : 0L,
                            ProcessingMessages = new ReleaseProcessingMessage[0]
                        };
                }

                return ExecuteInternalErmReleaseProcessing(acquiredRelease, organizationUnitDgppId, canIgnoreBlockingErrors);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Releasing aborted. Unexpected exception was caught. " +
                                        "Release details: for organization unit with stable (DGPP) id {0} by period {1} is beta = {2}. " +
                                        "Can ignore blocking errors: {3}",
                                        organizationUnitDgppId,
                                        period,
                                        isBeta,
                                        canIgnoreBlockingErrors);
                _tracer.Error(ex, msg);

                if (acquiredRelease != null)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                    {
                        _aggregateServiceIsolator.Execute<IReleaseChangeStatusAggregateService>(service => service.Finished(acquiredRelease, ReleaseStatus.Error, msg));
                        scope.Complete();
                    }
                }

                throw;
            }
        }

        private static bool IsValidReleaseStartArgs(TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors, out string report)
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

        private static ReleaseProcessingMessage[] AddNewBlockingMessage(ReleaseProcessingMessage[] processingMessages, string newMessage)
        {
            var extendedMessages = new ReleaseProcessingMessage[processingMessages.Length + 1];
            extendedMessages[0] = new ReleaseProcessingMessage { IsBlocking = true, Message = newMessage };

            processingMessages.CopyTo(extendedMessages, 1);

            return extendedMessages;
        }

        private bool TryAcquireTargetRelease(int organizationUnitDgppId,
                                             TimePeriod period,
                                             bool isBeta,
                                             bool canIgnoreBlockingErrors,
                                             out ReleaseInfo acquiredRelease,
                                             out long? previuosReleaseId,
                                             out string report)
        {
            acquiredRelease = null;
            previuosReleaseId = null;

            using (var scope = _scopeFactory.CreateNonCoupled<StartReleaseIdentity>())
            {
                _tracer.InfoFormat("Starting releasing for organization unit with stable (DGPP) id {0} by period {1} is beta = {2}. " +
                                     "Can ignore blocking errors: {3}",
                                     organizationUnitDgppId,
                                     period,
                                     isBeta,
                                     canIgnoreBlockingErrors);

                var organizationUnitId = _releaseReadModel.GetOrganizationUnitId(organizationUnitDgppId);
                if (organizationUnitId == 0)
                {
                    report = string.Format("Can't continue release. Organization unit with stable (DGPP) id {0} not found. " +
                                           "Release detail: {1} is beta = {2}. Can ignore blocking errors: {3}",
                                           organizationUnitDgppId,
                                           period,
                                           isBeta,
                                           canIgnoreBlockingErrors);
                    _tracer.Error(report);
                    return false;
                }

                var countryCode = _releaseReadModel.GetCountryCode(organizationUnitId);
                if (countryCode == 0)
                {
                    report = string.Format("Can't continue release. Country code for organization unit with id {0} not found. " +
                                           "Release detail: {1} is beta = {2}. Can ignore blocking errors: {3}",
                                           organizationUnitId,
                                           period,
                                           isBeta,
                                           canIgnoreBlockingErrors);
                    _tracer.Error(report);
                    return false;
                }

                if (!IsValidReleaseStartArgs(period, isBeta, canIgnoreBlockingErrors, out report))
                {
                    report = string.Format("Can't start releasing for organization unit with id {0} by period {1} is beta = {2}. " +
                                           "Can ignore blocking errors: {3}. Error description: {4}",
                                           organizationUnitId,
                                           period,
                                           isBeta,
                                           canIgnoreBlockingErrors,
                                           report);

                    _tracer.Error(report);
                    return false;
                }

                bool usingPreviouslyNotFinishedReleasing;
                if (!CanStartReleasing(organizationUnitId, period, isBeta, out usingPreviouslyNotFinishedReleasing, out previuosReleaseId, out report))
                {
                    report = string.Format("Can't start releasing for organization unit with id {0} by period {1} is beta = {2}. " +
                                           "Can ignore blocking errors: {3}. Error description: {4}",
                                           organizationUnitId,
                                           period,
                                           isBeta,
                                           canIgnoreBlockingErrors,
                                           report);

                    _tracer.Error(report);
                    return false;
                }

                if (!usingPreviouslyNotFinishedReleasing)
                {
                    _tracer.InfoFormat("Starting release for for organization unit with id {0} by period {1} is beta = {2}. Can ignore blocking errors: {3}",
                                         organizationUnitId,
                                         period,
                                         isBeta,
                                         canIgnoreBlockingErrors);

                    acquiredRelease = _releaseStartAggregateService.Start(countryCode,
                                                                          organizationUnitId,
                                                                          period,
                                                                          isBeta,
                                                                          ReleaseStatus.InProgressInternalProcessingStarted);
                }
                else
                {
                    var msg = string.Format("Using previously started release with id {0} for organization unit with id {1} by period {2} is beta = {3}, " +
                                            "that was not finished properly. Probably errors was detected on the external releasing side (Export and etc)",
                                            previuosReleaseId,
                                            organizationUnitId,
                                            period,
                                            isBeta);

                    _tracer.Info(msg);

                    // ReSharper disable once PossibleInvalidOperationException
                    acquiredRelease = _releaseReadModel.GetReleaseInfo(previuosReleaseId.Value);
                    _releaseChangeStatusAggregateService.InProgressInternalProcessingStarted(acquiredRelease, msg);
                }

                scope.Complete();
            }

            return true;
        }

        private bool CanStartReleasing(long organizationUnitId,
                                       TimePeriod period,
                                       bool isBeta,
                                       out bool usingPreviouslyNotFinishedReleasing,
                                       out long? previuosReleaseId,
                                       out string report)
        {
            var results = new List<Tuple<ReleaseStartingOption, ReleaseInfo>>();
            var releases = _releaseReadModel.GetReleasesInDescOrder(organizationUnitId, period);
            foreach (var conditionSet in _releaseStartingOptionConditionSets)
            {
                ReleaseInfo previousRelease;
                var releaseStartingOption = conditionSet.EvaluateStartingOption(isBeta, releases, out previousRelease);
                results.Add(Tuple.Create(releaseStartingOption, previousRelease));
            }

            foreach (var result in results)
            {
                var releaseStartingOption = result.Item1;
                var previousRelease = result.Item2;

                if (releaseStartingOption.HasFlag(ReleaseStartingOption.Denied))
                {
                    usingPreviouslyNotFinishedReleasing = false;
                    previuosReleaseId = null;

                    if (releaseStartingOption.HasFlag(ReleaseStartingOption.BecauseOfFinal))
                    {
                        previuosReleaseId = previousRelease.Id;
                        report = string.Format("Previous release with id {0} for organization unit with id {1} by period {2} is final and success status. " +
                                               "Can't start new release without reverting previous final and successful release",
                                               previousRelease.Id,
                                               organizationUnitId,
                                               period);
                        return false;
                    }

                    if (releaseStartingOption.HasFlag(ReleaseStartingOption.BecauseOfLock))
                    {
                        report = string.Format("Previous release with id {0} for organization unit with id {1} by period {2} has status {3}, " +
                                               "so new releasing can't be started",
                                               previousRelease.Id,
                                               organizationUnitId,
                                               period,
                                               previousRelease.Status);
                        return false;
                    }

                    if (releaseStartingOption.HasFlag(ReleaseStartingOption.BecauseOfInconsistency))
                    {
                        
                        report = string.Format("Incorrect system state detected for for organization unit with id {0} by period {1}. " +
                                               "There are two releases (one technical and one final) running in parallel is allowed only, " +
                                               "so new releasing can't be started",
                                               organizationUnitId,
                                               period);
                        return false;
                    }
                }

                if (releaseStartingOption.HasFlag(ReleaseStartingOption.Allowed))
                {
                    report = null;

                    if (releaseStartingOption.HasFlag(ReleaseStartingOption.New))
                    {
                        usingPreviouslyNotFinishedReleasing = false;
                        previuosReleaseId = null;
                        return true;
                    }

                    if (releaseStartingOption.HasFlag(ReleaseStartingOption.AsPrevious))
                    {
                        usingPreviouslyNotFinishedReleasing = true;
                        previuosReleaseId = previousRelease.Id;
                        return true;
                    }
                }
            }

            throw new ApplicationException(string.Format("Error occured while release starting for organization unit with id {0} by period {1}. " +
                                                         "Release starting option evaluation failed.",
                                                         organizationUnitId,
                                                         period));
        }

        private ReleaseStartingResult ExecuteInternalErmReleaseProcessing(ReleaseInfo acquiredRelease, int organizationUnitDgppId, bool canIgnoreBlockingErrors)
        {
            var releasingResult = new ReleaseStartingResult
                {
                    Succeed = false,
                    ReleaseId = acquiredRelease.Id,
                    ProcessingMessages = new ReleaseProcessingMessage[0]
                };

            // Forcing period to be set in UTC
            var releasingPeriod = new TimePeriod(new DateTime(acquiredRelease.PeriodStartDate.Ticks, DateTimeKind.Utc),
                                                 new DateTime(acquiredRelease.PeriodEndDate.Ticks, DateTimeKind.Utc));
            
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                if (!LockSuccessfullyAcquired(acquiredRelease))
                {
                    var msg = string.Format("Acquired release with id {0} for organization unit with id {1} by period {2} has processing status violations. " +
                                            "Aborting acquired release. " +
                                            "Possible reason for errors - concurrent release\reverting process and invalid release status processing",
                                            acquiredRelease.Id,
                                            acquiredRelease.OrganizationUnitId,
                                            releasingPeriod);
                    _releaseChangeStatusAggregateService.Finished(acquiredRelease, ReleaseStatus.Error, msg);

                    _tracer.Error(msg);
                    releasingResult.ProcessingMessages = new[] { new ReleaseProcessingMessage { IsBlocking = true, Message = msg } };

                    transaction.Complete();
                    return releasingResult;
                }

                var validationMessages = _validateOrdersForReleaseOperationService.Validate(acquiredRelease.OrganizationUnitId, releasingPeriod, acquiredRelease.IsBeta).ToArray();
                releasingResult.ProcessingMessages = validationMessages;

                bool hasBlockingErrors = validationMessages.Any(item => item.IsBlocking);
                _tracer.InfoFormat("Release with id {0} for organization unit with id {1} by period {2}. " +
                                     "After orders validation has blocking errors: {3} and can ingnore blocking errors: {4}",
                                     acquiredRelease.Id,
                                     acquiredRelease.OrganizationUnitId,
                                     releasingPeriod,
                                     hasBlockingErrors,
                                     canIgnoreBlockingErrors);

                if (hasBlockingErrors && !canIgnoreBlockingErrors)
                {
                    var msg = string.Format("Aborting release with id {0} for organization unit with id {1} by period {2}. " +
                                            "Has blocking errors that can't be ignored.",
                                            acquiredRelease.Id,
                                            acquiredRelease.OrganizationUnitId,
                                            releasingPeriod);

                    _tracer.Error(msg);
                    releasingResult.ProcessingMessages = AddNewBlockingMessage(releasingResult.ProcessingMessages, msg);

                    _releaseAttachProcessingMessagesAggregateService.SaveInternalMessages(acquiredRelease, validationMessages);
                    _releaseChangeStatusAggregateService.Finished(acquiredRelease, ReleaseStatus.Error, msg);

                    transaction.Complete();
                    return releasingResult;
                }

                if (!_ensureOrdersForReleaseCompletelyExportedOperationService.IsExported(acquiredRelease.Id,
                                                                                          acquiredRelease.OrganizationUnitId,
                                                                                          organizationUnitDgppId,
                                                                                          releasingPeriod,
                                                                                          acquiredRelease.IsBeta))
                {
                    var msg = string.Format("Releasing aborted. Ensure process detected that not all required orders for release are properly exported. " +
                                            "Release details: id {0} for organization unit with id {1} by period {2}",
                                            acquiredRelease.Id,
                                            acquiredRelease.OrganizationUnitId,
                                            releasingPeriod);
                    _tracer.Error(msg);
                    releasingResult.ProcessingMessages = AddNewBlockingMessage(releasingResult.ProcessingMessages, msg);

                    _releaseChangeStatusAggregateService.Finished(acquiredRelease, ReleaseStatus.Error, msg);

                    transaction.Complete();
                    return releasingResult;
                }

                releasingResult.Succeed = true;
                _releaseChangeStatusAggregateService.InProgressWaitingExternalProcessing(acquiredRelease);

                _tracer.InfoFormat("Release with id {0} successfully started for organization unit with id {1} by period {2} is beta = {3}. " +
                                     "Waiting for external release processing",
                                     acquiredRelease.Id,
                                     acquiredRelease.OrganizationUnitId,
                                     releasingPeriod,
                                     acquiredRelease.IsBeta);

                transaction.Complete();
            }

            return releasingResult;
        }

        private bool LockSuccessfullyAcquired(ReleaseInfo acquiredRelease)
        {
            // Проверяем, что pessimistic lock успешно захвачена,
            // для этого нужно убедиться, что после того как 
            // создали новую запись о сборке, или переоткрыли старую, 
            // из конкурирующей транзакции не захватили ту же пессимистичную блокировку (запись о сборке по тому же городу, за тот же период, в частном случае ту же запись о сборке)
            // т.к. есть режим подхвата сборки InProgressWaitingExternalProcessing, то доп. нужно проверить версию записи о сборке
            var releases = _releaseReadModel.GetReleasesInDescOrder(acquiredRelease.OrganizationUnitId,
                                                                    new TimePeriod(acquiredRelease.PeriodStartDate, acquiredRelease.PeriodEndDate));

            var runningReleases = releases.Where(x => x.Status == ReleaseStatus.InProgressInternalProcessingStarted &&
                                                      x.IsBeta == acquiredRelease.IsBeta)
                                          .ToArray();
            if (runningReleases.Count() > 1)
            {
                return false;
            }

            var lockedRelease = runningReleases.Single();
            return lockedRelease.Id == acquiredRelease.Id && acquiredRelease.SameVersionAs(lockedRelease);
        }
    }
}
