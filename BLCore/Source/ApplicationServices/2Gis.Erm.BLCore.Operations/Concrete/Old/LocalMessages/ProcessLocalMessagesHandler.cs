using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Xml;

using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Dgpp;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class ProcessLocalMessagesHandler : RequestHandler<ProcessLocalMessagesRequest, EmptyResponse>
    {
        private readonly IFileService _fileService;
        private readonly ILocalMessageRepository _localMessageRepository;
        private readonly ITracer _logger;
        private readonly ISubRequestProcessor _subRequestProcessor;

        public ProcessLocalMessagesHandler(ILocalMessageRepository localMessageRepository,
                                           IFileService fileService,
                                           ISubRequestProcessor subRequestProcessor,
                                           ITracer logger)
        {
            _localMessageRepository = localMessageRepository;
            _fileService = fileService;
            _subRequestProcessor = subRequestProcessor;
            _logger = logger;
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

                var exceptionMessage = (ex is XmlException) ? "Неверный формат сообщения" : ex.Message;
                var errorMessage = string.Format("Ошибка обработки сообщения [{0}]: {1}", localMessageDto.LocalMessage.Id, exceptionMessage);
                _logger.Error(ex, errorMessage);

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
                        "Cообщение загружено успешно [{0}]: было обработано [{1}] элементов из [{2}]",
                        localMessageDto.LocalMessage.Id,
                        response.Processed,
                        response.Total);
                _logger.Info(resultMessage);

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

            throw new NotificationException("Неподдерживаемый тип интеграционного запроса");
        }

        private ExportResponse ProcessExportRequest(LocalMessageDto localMessageDto)
        {
            var integrationType = (IntegrationTypeExport)localMessageDto.IntegrationType;
            switch (integrationType)
            {
                case IntegrationTypeExport.FirmsWithActiveOrdersToDgpp:
                    {
                        return ProcessFirmsWithActiveOrdersToDgpp(localMessageDto);
                    }

                case IntegrationTypeExport.DataForAutoMailer:
                    {
                        return ProcessDataForAutoMailer(localMessageDto);
                    }

                case IntegrationTypeExport.AccountDetailsToServiceBus:
                {
                    return ProcessAccountDetailsToServiceBus(localMessageDto);
                }

                case IntegrationTypeExport.LegalPersonsTo1C:
                case IntegrationTypeExport.None:
                    throw new NotificationException("Неподдерживаемый тип интеграционного запроса на экспорт");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ExportResponse ProcessAccountDetailsToServiceBus(LocalMessageDto localMessageDto)
        {
            var file = _fileService.GetFileById(localMessageDto.LocalMessage.FileId);

            var stream = file.Content.UnzipStream(x => Path.GetExtension(x) == ".xml").Single().Value;
            return (ExportResponse)_subRequestProcessor.HandleSubRequest(new WriteMessageToServiceBusRequest
                {
                    MessageStream = stream,
                    FlowName = "flowFinancialData",
                    XsdSchemaResourceExpression = () => Properties.Resources.flowFinancialData_DebitsInfoInitial
                },
                                                                         Context,
                                                                         false);
        }

        private ExportResponse ProcessFirmsWithActiveOrdersToDgpp(LocalMessageDto localMessageDto)
        {
            if (localMessageDto.LocalMessage.OrganizationUnitId == null)
            {
                throw new NotificationException(
                    string.Format("Для обработки сообщения типа '{0}' должен быть указан город", IntegrationTypeExport.FirmsWithActiveOrdersToDgpp));
            }

            var file = _fileService.GetFileById(localMessageDto.LocalMessage.FileId);
            var stream = file.Content;

            var response = (ExportResponse)_subRequestProcessor.HandleSubRequest(
                new WriteFirmsWithActiveOrdersToRabbitMqRequest
                {
                    MessageStream = stream,
                                                                                         OrganizationUnitId =
                                                                                             localMessageDto.LocalMessage.OrganizationUnitId.Value,
                },
                Context,
                false);

            return response;
        }

        private ExportResponse ProcessDataForAutoMailer(LocalMessageDto localMessageDto)
        {
            var file = _fileService.GetFileById(localMessageDto.LocalMessage.FileId);
            var stream = file.Content;

            return (ExportResponse)_subRequestProcessor.HandleSubRequest(new WriteMessageToServiceBusRequest
            {
                MessageStream = stream,
                FlowName = "flowDeliveryData",
                    XsdSchemaResourceExpression = () => Properties.Resources.flowDeliveryData_SendingGroup
            },
                                                             Context,
                                                             false);
        }

        private ImportResponse ProcessImportRequest(LocalMessageDto localMessageDto)
        {
            ImportResponse response;

            var file = _fileService.GetFileById(localMessageDto.LocalMessage.FileId);
            var stream = file.Content;

            var integrationType = (IntegrationTypeImport)localMessageDto.IntegrationType;
            switch (integrationType)
            {
                case IntegrationTypeImport.FirmsFromDgpp:
                    {
                        response = (ImportResponse)_subRequestProcessor.HandleSubRequest(
                            new DgppImportFirmsRequest { MessageStream = stream },
                            Context,
                            false);
                        break;
                    }

                case IntegrationTypeImport.TerritoriesFromDgpp:
                    {
                        response = (ImportResponse)_subRequestProcessor.HandleSubRequest(
                            new DgppImportTerritoriesRequest { MessageStream = stream },
                            Context,
                            false);
                        break;
                    }

                case IntegrationTypeImport.AccountDetailsFrom1C:
                    {
                        response = (ImportResponse)_subRequestProcessor.HandleSubRequest(
                                                                                     new ImportAccountDetailsFrom1CRequest
                                                                                         {
                                                                                             InputStream = stream,
                                                                                             FileName = localMessageDto.FileName
                                                                                         },
                            Context,
                            false);
                        break;
                    }

                default:
                    throw new NotificationException("Неподдерживаемый тип интеграционного запроса на импорт");
            }

            localMessageDto.LocalMessage.OrganizationUnitId = response.OrganizationUnitId;

            return response;
        }
    }
}