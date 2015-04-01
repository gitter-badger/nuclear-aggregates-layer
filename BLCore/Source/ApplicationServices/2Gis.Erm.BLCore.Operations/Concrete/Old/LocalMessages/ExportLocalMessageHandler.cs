using System;
using System.IO;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class ExportLocalMessageHandler : RequestHandler<ExportLocalMessageRequest, Response>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ExportLocalMessageHandler(
            ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override Response Handle(ExportLocalMessageRequest request)
        {
            Response response = null;
            var successFlag = true;
            var errorMsg = string.Empty;
            BusinessLogicException ex2 = null;
            var statusOnSuccess = LocalMessageStatus.WaitForProcess;
            if (request.IntegrationType == IntegrationTypeExport.LegalPersonsTo1C)
            {
                statusOnSuccess = LocalMessageStatus.Processed;
            }

            try
            {
                response = ProcessExportMessage(request);
            }
            catch (BusinessLogicException ex)
            {
                successFlag = false;
                errorMsg = ex.Message;
                ex2 = new BusinessLogicException(string.Format(BLResources.ExportLocalMessageError, ex.Message), ex);
            }

            var integrationResponse = response as IntegrationResponse;

            var haveErrors = !successFlag || (integrationResponse != null && integrationResponse.BlockingErrorsAmount > 0);

            // fix 3451, но нужна переработка локальных сообщений в части экспорта
            // не должно создаваться пустое сообщение с ошибкой - нужно просто логгировать ошибку, так же как сделано везде в системе
            // иными словать нижний if должен полностью удалиться
            if (!(response is EmptyResponse))
            {
                _subRequestProcessor
                    .HandleSubRequest(new CreateLocalMessageRequest
                        {
                            Content = successFlag && integrationResponse != null ? integrationResponse.Stream : new MemoryStream(),
                            IntegrationType = (int)request.IntegrationType,
                            FileName = successFlag && integrationResponse != null ? integrationResponse.FileName : string.Empty,
                            ContentType = successFlag && integrationResponse != null ? integrationResponse.ContentType : string.Empty,
                            Entity =
                                new LocalMessage
                                    {
                                        EventDate = DateTime.UtcNow,
                                        Status = !haveErrors ? statusOnSuccess : LocalMessageStatus.Failed,
                                        OrganizationUnitId = request.OrganizationUnitId,
                                        ProcessResult = successFlag && integrationResponse != null
                                                ? (integrationResponse.BlockingErrorsAmount > 0
                                                   || integrationResponse.NonBlockingErrorsAmount > 0
                                                       ? BLResources.ErrorsAreInTheFile
                                                       : string.Empty)
                                                : errorMsg
                                    }

                            // Если не sucсessFlag, то предыдущая транзакция была закрыта при выкидывании 
                            // BusinessLogicException. Поэтому открываем новую.
                        },
                    Context,
                    successFlag);
            }

            if (!successFlag)
            {
                throw ex2;
            }

            return response;
        }

        private Response ProcessExportMessage(ExportLocalMessageRequest request)
        {
            switch (request.IntegrationType)
            {
                case IntegrationTypeExport.LegalPersonsTo1C:
                    {
                        return _subRequestProcessor.HandleSubRequest(
                            new ExportLegalPersonsRequest
                                {
                                    OrganizationUnitId = request.OrganizationUnitId,
                                    PeriodStart = request.PeriodStart
                                },
                            Context);
                    }

                case IntegrationTypeExport.DataForAutoMailer:
                    {
                        return _subRequestProcessor.HandleSubRequest(
                            new ExportDataForAutoMailerRequest
                                {
                                    SendingType = request.SendingType,
                                    PeriodStart = request.PeriodStart,
                                    IncludeRegionalAdvertisement = request.IncludeRegionalAdvertisement
                                },
                            Context);
                    }

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
