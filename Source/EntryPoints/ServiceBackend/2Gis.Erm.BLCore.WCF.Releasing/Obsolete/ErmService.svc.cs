using System;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Obsolete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.WCF.Releasing.Obsolete
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ErmService : IErmService
    {
        private readonly IStartReleaseOperationService _startReleaseOperationService;
        private readonly IAttachExternalReleaseProcessingMessagesOperationService _attachExternalReleaseProcessingMessagesOperationService;
        private readonly IFinishReleaseOperationService _finishReleaseOperationService;
        private readonly ICommonLog _logger;

        public ErmService(IStartReleaseOperationService startReleaseOperationService,
                          IAttachExternalReleaseProcessingMessagesOperationService attachExternalReleaseProcessingMessagesOperationService,
                          IFinishReleaseOperationService finishReleaseOperationService,
                          IUserContext userContext,
                          ICommonLog logger)
        {
            _startReleaseOperationService = startReleaseOperationService;
            _attachExternalReleaseProcessingMessagesOperationService = attachExternalReleaseProcessingMessagesOperationService;
            _finishReleaseOperationService = finishReleaseOperationService;
            _logger = logger;

            BLResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            MetadataResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            EnumResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public Response Handle(Request request)
        {
            var startExportRequest = request as StartExportRequest;
            if (startExportRequest != null)
            {
                return StartExport(startExportRequest);
            }

            var finishExportRequest = request as FinishExportRequest;
            if (finishExportRequest != null)
            {
                return FinishExport(finishExportRequest);
            }

            var notSupportedExceptionMessage = string.Format("Request of type '{0}' is unknown", request.GetType().Name);
            throw new FaultException<NotSupportedException>(new NotSupportedException(notSupportedExceptionMessage), new FaultReason(notSupportedExceptionMessage));
        }

        private StartExportResponse StartExport(StartExportRequest startExportRequest)
        {
            var organizationUnitDgppId = startExportRequest.OrganizationUnitId;
            var period = new TimePeriod(startExportRequest.PeriodStart, startExportRequest.PeriodEnd);
            var isBeta = startExportRequest.IsTechnical;
            var canIgnoreBlockingErrors = startExportRequest.IgnoreBlockingErrors;

            try
            {
                var result = _startReleaseOperationService.Start(organizationUnitDgppId, period, isBeta, canIgnoreBlockingErrors);
                return new StartExportResponse
                    {
                        ReleaseInfoId = result.ReleaseId,
                        ValidationResults = result.ProcessingMessages
                                                  .Select(x => new ValidationResult
                                                      {
                                                          IsBlocking = x.IsBlocking,
                                                          Message = x.Message,
                                                          OrderId = x.OrderId,
                                                          OrderNumber = x.OrderNumber,
                                                          RuleCode = x.RuleCode
                                                      })
                                                  .ToArray()
                    };
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex,
                                      "Can't start release for organization unit with stable (DGPP) id {0} by period {1} is beta {2}. Can ignore blocking errors: {3}",
                                      organizationUnitDgppId,
                                      period,
                                      isBeta,
                                      canIgnoreBlockingErrors);
                throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
            }
        }

        private FinishExportResponse FinishExport(FinishExportRequest finishExportRequest)
        {
            var releaseId = finishExportRequest.ReleaseInfoId;
            var externalMessages = finishExportRequest.ValidationErrors
                                                      .Select(x => new ExternalReleaseProcessingMessage
                                                          {
                                                              IsBlocking = x.IsBlocking,
                                                              MessageType = x.RuleCode,
                                                              Description = x.Message
                                                          })
                                                      .ToArray();
            if (finishExportRequest.IsSuccessed)
            {
                try
                {
                    _finishReleaseOperationService.Succeeded(finishExportRequest.ReleaseInfoId);
                    _attachExternalReleaseProcessingMessagesOperationService.Attach(releaseId, externalMessages);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex,
                                          "Can't finish release properly with succeeded result. Release id: {0}",
                                          releaseId);

                    throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
                }
            }
            else
            {
                try
                {
                    _finishReleaseOperationService.Failed(releaseId);
                    _attachExternalReleaseProcessingMessagesOperationService.Attach(releaseId, externalMessages);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex,
                                          "Can't finish release properly with failed result. Release id: {0}",
                                          releaseId);

                    throw new FaultException<ReleasingErrorDescription>(new ReleasingErrorDescription(ex.Message), new FaultReason(ex.Message));
                }
            }

            return new FinishExportResponse();
        }
    }
}