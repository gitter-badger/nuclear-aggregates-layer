using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class StartSimplifiedReleaseOperationService : IStartSimplifiedReleaseOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IEnumerable<IReleaseStartingOptionConditionSet> _releaseStartingOptionConditionSets;
        private readonly IReleaseStartAggregateService _releaseStartAggregateService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public StartSimplifiedReleaseOperationService(IOrderReadModel orderReadModel,
                                                      IReleaseReadModel releaseReadModel,
                                                      // ReSharper disable once ParameterTypeCanBeEnumerable.Local
                                                      IReleaseStartingOptionConditionSet[] releaseStartingOptionConditionSets,
                                                      IReleaseStartAggregateService releaseStartAggregateService,
                                                      ISecurityServiceFunctionalAccess functionalAccessService,
                                                      IUserContext userContext,
                                                      IOperationScopeFactory scopeFactory,
                                                      ITracer tracer)
        {
            _orderReadModel = orderReadModel;
            _releaseReadModel = releaseReadModel;
            _releaseStartingOptionConditionSets = releaseStartingOptionConditionSets;
            _releaseStartAggregateService = releaseStartAggregateService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public ReleaseStartingResult Start(long organizationUnitId, TimePeriod period, bool isBeta)
        {
            var releaseInitiator = _userContext.Identity.Code;
            var releaseDescriptor = new ReleaseStartingResult { ProcessingMessages = new ReleaseProcessingMessage[0] };

            using (var scope = _scopeFactory.CreateNonCoupled<StartSimplifiedReleaseIdentity>())
            {
                _tracer.InfoFormat("Starting releasing (simplified mode) for organization unit with id {0} by period {1} is beta {2}. " +
                                     "Release initiator user id {3}",
                                     organizationUnitId,
                                     period,
                                     isBeta,
                                     releaseInitiator);

                int countryCode;
                string report;
                if (!CanStartRelease(organizationUnitId, period, isBeta, out countryCode, out report))
                {
                    _tracer.ErrorFormat("Can't start simlified releasing for organization unit with id {0} by period {1} is beta {2}. " +
                                          "Release initiator user id {3}. Error: {4}",
                                          organizationUnitId,
                                          period,
                                          isBeta,
                                          releaseInitiator,
                                          report);
                    throw new NotificationException(report);
                }

                var startedRelease = _releaseStartAggregateService.Start(countryCode,
                                                                         organizationUnitId,
                                                                         period,
                                                                         isBeta,
                                                                         ReleaseStatus.InProgressWaitingExternalProcessing);
                releaseDescriptor.ReleaseId = startedRelease.Id;
                releaseDescriptor.Succeed = true;

                _tracer.InfoFormat("Simplified release with id {0} successfully started for organization unit with id {1} by period {2} is beta {3}. " +
                                     "Release initiator user id {4}",
                                     startedRelease.Id,
                                     organizationUnitId,
                                     period,
                                     isBeta,
                                     releaseInitiator);

                scope.Complete();
            }

            return releaseDescriptor;
        }

        private bool CanStartRelease(long organizationUnitId, TimePeriod period, bool isBeta, out int countryCode, out string report)
        {
            countryCode = 0;
            report = null;

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, _userContext.Identity.Code))
            {
                report = "User doesn't have sufficient privileges for managing releasing in simplified mode";
                return false;
            }

            countryCode = _releaseReadModel.GetCountryCode(organizationUnitId);
            if (countryCode == 0)
            {
                report = string.Format("Can't continue release (simplified mode). Country code for organization unit with id {0} not found",
                                       organizationUnitId);
                return false;
            }

            var releaseStartingOption = ReleaseStartingOption.Undifined;
            var releases = _releaseReadModel.GetReleasesInDescOrder(organizationUnitId, period);
            foreach (var conditionSet in _releaseStartingOptionConditionSets)
            {
                ReleaseInfo previousRelease;
                releaseStartingOption |= conditionSet.EvaluateStartingOption(isBeta, releases, out previousRelease);
            }

            if (releaseStartingOption.HasFlag(ReleaseStartingOption.Denied))
            {
                report = releaseStartingOption.HasFlag(ReleaseStartingOption.BecauseOfFinal)
                             ? BLResources.CannotStartReleaseBecausePreviousReleaseWasFinal
                             : BLResources.CannotStartReleaseBecauseAnotherReleaseIsRunning;

                return false;
            }

            if (!_orderReadModel.GetOrderIdsForRelease(organizationUnitId, period).Any())
            {
                report = BLResources.CannotStartReleaseBecauseNoOrdersIsFoundForRelease;
                return false;
            }

            var isReleaseMustBeLaunchedThroughExport = _releaseReadModel.IsReleaseMustBeLaunchedThroughExport(organizationUnitId);
            if (isReleaseMustBeLaunchedThroughExport)
            {
                report = BLResources.CannotInstantStartRelease_UseExportClient;
                return false;
            }

            return true;
        }
    }
}