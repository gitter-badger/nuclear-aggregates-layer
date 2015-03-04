using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Releasing
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class ReleaseApplicationService : IReleaseApplicationService
    {
        private readonly IStartReleaseOperationService _startReleaseOperationService;
        private readonly IAttachExternalReleaseProcessingMessagesOperationService _attachExternalReleaseProcessingMessagesOperationService;
        private readonly IFinishReleaseOperationService _finishReleaseOperationService;
        private readonly ICommonLog _logger;

        public ReleaseApplicationService(IStartReleaseOperationService startReleaseOperationService,
                                         IAttachExternalReleaseProcessingMessagesOperationService attachExternalReleaseProcessingMessagesOperationService,
                                         IFinishReleaseOperationService finishReleaseOperationService,
                                         IUserContext userContext,
                                         IResourceGroupManager resourceGroupManager,
                                         ICommonLog logger)
        {
            _startReleaseOperationService = startReleaseOperationService;
            _attachExternalReleaseProcessingMessagesOperationService = attachExternalReleaseProcessingMessagesOperationService;
            _finishReleaseOperationService = finishReleaseOperationService;
            _logger = logger;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public ReleaseStartingResult Start(int organizationUnitDgppId, TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors)
        {
            try
            {
                return _startReleaseOperationService.Start(organizationUnitDgppId, period, isBeta, canIgnoreBlockingErrors);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex,
                                      "Can't start release for organization unit with stable (DGPP) id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}",
                                      organizationUnitDgppId,
                                      period,
                                      isBeta,
                                      canIgnoreBlockingErrors);
                throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
            }
        }

        public void Attach(long releaseId, ExternalReleaseProcessingMessage[] messages)
        {
            try
            {
                _attachExternalReleaseProcessingMessagesOperationService.Attach(releaseId, messages);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex,
                                      "Can't attach external release processing messages. Release id: {0}",
                                      releaseId);

                throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
            }
        }

        public void Succeeded(long releaseId)
        {
            try
            {
                _finishReleaseOperationService.Succeeded(releaseId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex,
                                      "Can't finish release properly with succeeded result. Release id: {0}",
                                      releaseId);

                throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
            }
        }

        public void Failed(long releaseId)
        {
            try
            {
                _finishReleaseOperationService.Failed(releaseId);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex,
                                      "Can't finish release properly with failed result. Release id: {0}",
                                      releaseId);

                throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
            }
        }
    }
}
