using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Transactions;
using System.Xml;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class ProcessLocalMessagesHandler : RequestHandler<ProcessLocalMessagesRequest, EmptyResponse>
    {
        private readonly IFileService _fileService;
        private readonly ILocalMessageRepository _localMessageRepository;
        private readonly ITracer _tracer;
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ProcessLocalMessagesHandler(ILocalMessageRepository localMessageRepository,
                                           IFileService fileService,
                                           ISubRequestProcessor subRequestProcessor,
                                           ITracer tracer)
        {
            _localMessageRepository = localMessageRepository;
            _fileService = fileService;
            _subRequestProcessor = subRequestProcessor;
            _tracer = tracer;
        }

        protected override EmptyResponse Handle(ProcessLocalMessagesRequest request)
        {
            LocalMessageDto localMessageDto;
            do
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    localMessageDto = _localMessageRepository.GetMessageToProcess();

                    if (localMessageDto != null)
                    {
                        _localMessageRepository.SetProcessingState(localMessageDto.LocalMessage);
                    }

                    transaction.Complete();
                }

                if (localMessageDto != null)
                {
                    ProcessMessageInternal(localMessageDto);
                }
            }
            while (localMessageDto != null);

            return Response.Empty;
        }

        private void ProcessMessageInternal(LocalMessageDto localMessageDto)
        {
            var stopwatch = Stopwatch.StartNew();
            LocalMessageStatus status;
            var messages = new List<string>();

            try
            {
                ProcessMessage(localMessageDto, messages);
                status = LocalMessageStatus.Processed;
            }
            catch (Exception ex)
            {
                status = LocalMessageStatus.Failed;

                var exceptionMessage = (ex is XmlException) ? "�������� ������ ���������" : ex.Message;
                var errorMessage = string.Format("������ ��������� ��������� [{0}]: {1}", localMessageDto.LocalMessage.Id, exceptionMessage);
                _tracer.Error(ex, errorMessage);

                messages.Add(errorMessage);
            }

            stopwatch.Stop();
            _localMessageRepository.SetResult(localMessageDto.LocalMessage, status, messages, stopwatch.ElapsedMilliseconds);
        }

        private void ProcessMessage(LocalMessageDto localMessageDto, List<string> messages)
        {
            if (Enum.IsDefined(typeof(IntegrationTypeImport), localMessageDto.IntegrationType))
            {
                var response = ProcessImportRequest(localMessageDto);

                var resultMessage =
                    string.Format(
                        "C�������� ��������� ������� [{0}]: ���� ���������� [{1}] ��������� �� [{2}]",
                        localMessageDto.LocalMessage.Id,
                        response.Processed,
                        response.Total);
                _tracer.Info(resultMessage);

                messages.Add(resultMessage);
                if (response.Messages != null)
                {
                    messages.AddRange(response.Messages);
                }

                return;
            }

            if (Enum.IsDefined(typeof(IntegrationTypeExport), localMessageDto.IntegrationType))
            {
                var response = ProcessExportRequest(localMessageDto);
                if (response.Messages != null)
                {
                    messages.AddRange(response.Messages);
                }

                return;
            }

            throw new NotificationException("���������������� ��� ��������������� �������");
        }

        private ExportResponse ProcessExportRequest(LocalMessageDto localMessageDto)
        {
            var integrationType = (IntegrationTypeExport)localMessageDto.IntegrationType;
            switch (integrationType)
            {
                case IntegrationTypeExport.DataForAutoMailer:
                    {
                        return ProcessDataForAutoMailer(localMessageDto);
                    }

                case IntegrationTypeExport.LegalPersonsTo1C:
                case IntegrationTypeExport.None:
                    throw new NotificationException("���������������� ��� ��������������� ������� �� �������");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        private ExportResponse ProcessDataForAutoMailer(LocalMessageDto localMessageDto)
        {
            var file = _fileService.GetFileById(localMessageDto.LocalMessage.FileId);
            var stream = file.Content;

            var subrequest = new WriteMessageToServiceBusRequest
            {
                MessageStream = stream,
                FlowName = "flowDeliveryData",
                    XsdSchemaResourceExpression = () => Properties.Resources.flowDeliveryData_SendingGroup
                };
            return (ExportResponse)_subRequestProcessor.HandleSubRequest(subrequest, Context, false);
        }

        private ImportResponse ProcessImportRequest(LocalMessageDto localMessageDto)
        {
            var file = _fileService.GetFileById(localMessageDto.LocalMessage.FileId);
            var integrationType = (IntegrationTypeImport)localMessageDto.IntegrationType;
            var response = GetResponse(integrationType, file.Content, localMessageDto.FileName);
            localMessageDto.LocalMessage.OrganizationUnitId = response.OrganizationUnitId;
            return response;
        }

        private ImportResponse GetResponse(IntegrationTypeImport integrationType, Stream stream, string fileName)
        {
            return (ImportResponse)_subRequestProcessor.HandleSubRequest(
                CreateRequest(integrationType, stream, fileName),
                Context,
                false);
        }

        private Request CreateRequest(IntegrationTypeImport integrationType, Stream stream, string fileName)
        {
            switch (integrationType)
            {
                case IntegrationTypeImport.AccountDetailsFrom1C:
                    return new ImportAccountDetailsFrom1CRequest
                                                                                         {
                                                                                             InputStream = stream,
                                   FileName = fileName
                               };

                default:
                    throw new NotificationException("���������������� ��� ��������������� ������� �� ������");
            }
        }
    }
}