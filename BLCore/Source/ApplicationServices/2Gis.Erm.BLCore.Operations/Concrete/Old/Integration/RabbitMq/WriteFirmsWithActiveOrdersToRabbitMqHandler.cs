using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.CorporateQueue.RabbitMq;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.RabbitMq
{
    public sealed class WriteFirmsWithActiveOrdersToRabbitMqHandler : RequestHandler<WriteFirmsWithActiveOrdersToRabbitMqRequest, ExportResponse>
    {
        private readonly ITracer _tracer;
        private readonly IFirmRepository _firmRepository;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IRabbitMqQueueFactory _rabbitMqQueueFactory;

        public WriteFirmsWithActiveOrdersToRabbitMqHandler(IRabbitMqQueueFactory rabbitMqQueueFactory, IIntegrationSettings integrationSettings, IFirmRepository firmRepository, ITracer tracer)
        {
            _rabbitMqQueueFactory = rabbitMqQueueFactory;
            _integrationSettings = integrationSettings;
            _firmRepository = firmRepository;
            _tracer = tracer;
        }

        protected override ExportResponse Handle(WriteFirmsWithActiveOrdersToRabbitMqRequest request)
        {
            var respоnse = new ExportResponse();

            if (!_integrationSettings.EnableRabbitMqQueue)
            {
                return respоnse;
            }

            var organizationUnitDgppId = _firmRepository.GetOrganizationUnitDgppId(request.OrganizationUnitId);

            if (organizationUnitDgppId == 0)
            {
                throw CreateAndLogFatalError("Невозможно найти отделение организации с идентификатором '{0}'", request.OrganizationUnitId);
            }

            const string exchangeName = "erm2dgpp";
            var writer = _rabbitMqQueueFactory.CreateQueueWriter(exchangeName);
            using (writer)
            {
                var stream = request.MessageStream;
                var messageBody = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(messageBody, 0, (int)stream.Length);

                var routingKey = string.Format("firmsWithOrders.{0}", organizationUnitDgppId);
                writer.Write(routingKey, messageBody);

                respоnse.Messages = new[] { string.Format("Сообщение успешно выгружено в RabbitMq, exchange='{0}', routingKey='{1}'", exchangeName, routingKey) };
            }

            return respоnse;
        }

        private NotificationException CreateAndLogFatalError(string format, params object[] args)
        {
            var message = string.Format(format, args);
            _tracer.Fatal(message);
            throw new NotificationException(message);
        }
    }
}
