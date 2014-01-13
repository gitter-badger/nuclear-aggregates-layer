using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class StartSimplifiedReleaseOperationService : IStartSimplifiedReleaseOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IReleaseStartAggregateService _releaseStartAggregateService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public StartSimplifiedReleaseOperationService(
            IOrderReadModel orderReadModel,
            IReleaseReadModel releaseReadModel,
            IReleaseStartAggregateService releaseStartAggregateService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _orderReadModel = orderReadModel;
            _releaseReadModel = releaseReadModel;
            _releaseStartAggregateService = releaseStartAggregateService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public ReleaseStartingResult Start(long organizationUnitId, TimePeriod period, bool isBeta)
        {
            var releaseInitiator = _userContext.Identity.Code;
            var releaseDescriptor = new ReleaseStartingResult { ProcessingMessages = new ReleaseProcessingMessage[0] };

            using (var scope = _scopeFactory.CreateNonCoupled<StartSimplifiedReleaseIdentity>())
            {
                _logger.InfoFormatEx(
                    "Starting releasing (simplified mode) for organization unit with id {0} by period {1} is beta {2}. Release initiator user id {3}", 
                    organizationUnitId, 
                    period, 
                    isBeta, 
                    releaseInitiator);

                string report;
                if (!CanStartRelease(organizationUnitId, period, out report))
                {
                    _logger.ErrorFormatEx(
                        "Can't start simlified releasing for organization unit with id {0} by period {1} is beta {2}. Release initiator user id {3}. Error: {4}", 
                        organizationUnitId, 
                        period, 
                        isBeta,
                        releaseInitiator,
                        report);
                    throw new NotificationException(report);
                }

                var startedRelease = _releaseStartAggregateService.Start(organizationUnitId, period, isBeta, ReleaseStatus.InProgressWaitingExternalProcessing);
                releaseDescriptor.ReleaseId = startedRelease.Id;
                releaseDescriptor.Succeed = true;

                _logger.InfoFormatEx(
                    "Simplified release with id {0} successfully started for organization unit with id {1} by period {2} is beta {3}. Release initiator user id {4}",
                    startedRelease.Id,
                    organizationUnitId, 
                    period, 
                    isBeta, 
                    releaseInitiator);

                scope.Complete();
            }

            return releaseDescriptor;
        }

        private bool CanStartRelease(long organizationUnitId, TimePeriod period, out string report)
        {
            report = null;

            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, _userContext.Identity.Code))
            {
                report = "User doesn't have sufficient privileges for managing releasing in simplified mode";
                return false;
            }

            var previousReleaseInfo = _releaseReadModel.GetLastRelease(organizationUnitId, period);
            if (previousReleaseInfo != null)
            {
                switch ((ReleaseStatus)previousReleaseInfo.Status)
                {
                    case ReleaseStatus.InProgressInternalProcessingStarted:
                    {
                        report = BLResources.CannotStartReleaseBecauseAnotherReleaseIsRunning;
                        return false;
                    }
                    case ReleaseStatus.InProgressWaitingExternalProcessing:
                    {
                        report = BLResources.CannotStartReleaseBecauseAnotherReleaseIsRunning;
                        return false;
                    }
                    case ReleaseStatus.Success:
                    {
                        if (!previousReleaseInfo.IsBeta)
                        {
                            report = BLResources.CannotStartReleaseBecausePreviousReleaseWasFinal;
                            return false;
                        }

                        break;
                    }
                }
            }

            if (!_orderReadModel.GetOrdersForRelease(organizationUnitId, period).Any())
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