using System;
using System.Globalization;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.RabbitMq
{
    public sealed class ImportlocalMessagesFromRabbitMqHandler : RequestHandler<ImportLocalMessagesFromRabbitMqRequest, EmptyResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly ICommonLog _logger;
        private readonly IRabbitMqQueueFactory _corporateQueueFactory;

        public ImportlocalMessagesFromRabbitMqHandler(ISubRequestProcessor subRequestProcessor, IIntegrationSettings integrationSettings, IRabbitMqQueueFactory corporateQueueFactory, ICommonLog logger)
        {
            _subRequestProcessor = subRequestProcessor;
            _integrationSettings = integrationSettings;
            _logger = logger;
            _corporateQueueFactory = corporateQueueFactory;
        }

        protected override EmptyResponse Handle(ImportLocalMessagesFromRabbitMqRequest request)
        {
            if (!_integrationSettings.EnableRabbitMqQueue)
            {
                return Response.Empty;
            }

            var reader = _corporateQueueFactory.CreateQueueReader(request.QueueName);
            if (reader == null)
            {
                throw new NotificationException(string.Format(CultureInfo.CurrentCulture, "Очередь '{0}' не найдена", request.QueueName));
            }

            using (reader)
            {
                try
                {
                    foreach (var corporateMessage in reader.ReadAndRemove())
                    {
                        _logger.Info(string.Format("Принято новое сообщение из корпоративной очереди [{0}], [{1}]", request.QueueName, corporateMessage));

                        _subRequestProcessor.HandleSubRequest(new CreateLocalMessageRequest
                        {
                            Content = corporateMessage.CreateStream(),
                                IntegrationType = (int) request.IntegrationType,

                            FileName = "IntegrationMessage",
                            ContentType = MediaTypeNames.Application.Octet,

                            Entity = new LocalMessage
                            {
                                EventDate = DateTime.UtcNow,
                                        Status = LocalMessageStatus.WaitForProcess,
                            }
                        }, Context);

                        _logger.Info(string.Format("Сообщение из корпоративной очереди [{0}], успешно перемещено в локальную очередь, [{1}]", request.QueueName, corporateMessage));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, String.Format("Произошла ошибка при загрузке сообщения из очереди [{0}]", request.QueueName));
                }
            }

            return Response.Empty;
        }
    }
}
