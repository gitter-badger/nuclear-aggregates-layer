using System;
using System.Collections.Generic;
using System.IO;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf;
using DoubleGis.Erm.Platform.Model.Entities;

using Microsoft.ServiceBus.Messaging;

using NuClear.Tracing.API;

using ProtoBuf.Meta;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public sealed class BinaryEntireTrackedUseCase2BrokeredMessageConverter : ITrackedUseCase2BrokeredMessageConverter
    {
        private readonly ITracer _tracer;
        private readonly RuntimeTypeModel _protobufModel;

        public BinaryEntireTrackedUseCase2BrokeredMessageConverter(ITracer tracer)
        {
            _tracer = tracer;
            _protobufModel = ProtoBufTypeModelForTrackedUseCaseConfigurator.Configure();
        }

        public IEnumerable<BrokeredMessage> Convert(TrackedUseCase useCase)
        {
            var stream = new MemoryStream();
            try
            {
                _protobufModel.Serialize(stream, useCase);
                stream.Position = 0;
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex,
                                      "Can't serialize tracked use case to brokered message. Use case description: {0}",
                                      useCase);
                stream.Dispose();
                throw;
            }

            var message = new BrokeredMessage(stream, true);
            message.Properties.Add(TrackedUseCaseMessageProperties.Indicator.Name, TrackedUseCaseMessageProperties.Indicator.Value);
            message.Properties.Add(TrackedUseCaseMessageProperties.Names.MessageBodyType, (int)MessageBodyType.Binary);
            message.Properties.Add(TrackedUseCaseMessageProperties.Names.FormatVersion, (int)TrackedUseCaseMessageFormatVersion.V1Entire);
            message.Properties.Add(TrackedUseCaseMessageProperties.Names.Operation, useCase.RootNode.OperationIdentity.OperationIdentity.Id);
            message.Properties.Add(TrackedUseCaseMessageProperties.Names.EntitiesSetHash, useCase.RootNode.OperationIdentity.Entities.EvaluateHash());
            message.Properties.Add(TrackedUseCaseMessageProperties.Names.UseCaseId, useCase.RootNode.ScopeId);

            return new[] { message };
        }
    }
}