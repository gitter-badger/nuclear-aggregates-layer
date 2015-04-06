using System;
using System.IO;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf;

using Microsoft.ServiceBus.Messaging;

using NuClear.Tracing.API;

using ProtoBuf.Meta;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer
{
    public sealed class BinaryEntireBrokeredMessage2TrackedUseCaseTransformer<TMessageFlow> : 
        MessageTransformerBase<TMessageFlow, ServiceBusPerformedOperationsMessage, TrackedUseCase>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly ITracer _tracer;
        private readonly RuntimeTypeModel _protobufModel;

        public BinaryEntireBrokeredMessage2TrackedUseCaseTransformer(ITracer tracer)
        {
            _tracer = tracer;
            _protobufModel = ProtoBufTypeModelForTrackedUseCaseConfigurator.Configure();
        }

        protected override TrackedUseCase Transform(ServiceBusPerformedOperationsMessage originalMessage)
        {
            BrokeredMessage targetMessage = null;

            try
            {
                targetMessage = originalMessage.Operations.Single();
                var messageBody = targetMessage.GetBody<Stream>();
                messageBody.Position = 0;

                var useCase = (TrackedUseCase)_protobufModel.Deserialize(messageBody, null, typeof(TrackedUseCase));
                useCase.Tracker.SynchronizeAuxiliaryData();
                useCase.Tracker.Complete();
                return useCase;
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex,
                                      "Can't deserialize tracked use case from brokered message. Message desciption: {0}",
                                      targetMessage != null ? targetMessage.ToString() : "Message instance is null");
                throw;
            }
        }
    }
}