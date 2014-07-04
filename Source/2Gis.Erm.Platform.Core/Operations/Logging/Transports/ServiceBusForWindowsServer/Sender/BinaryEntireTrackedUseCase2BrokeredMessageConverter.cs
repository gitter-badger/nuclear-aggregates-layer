using System;
using System.Collections.Generic;
using System.IO;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Serialization.ProtoBuf;
using DoubleGis.Erm.Platform.Model.Entities;

using Microsoft.ServiceBus.Messaging;

using ProtoBuf.Meta;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public sealed class BinaryEntireTrackedUseCase2BrokeredMessageConverter : ITrackedUseCase2BrokeredMessageConverter
    {
        private readonly ICommonLog _logger;
        private readonly RuntimeTypeModel _protobufModel;

        public BinaryEntireTrackedUseCase2BrokeredMessageConverter(ICommonLog logger)
        {
            _logger = logger;
            _protobufModel = ProtoBufTypeModelForTrackedUseCaseConfigurator.Configure();

            //var configuredSchema = _protobufModel.GetSchema(typeof(TestClass));//TrackedUseCase));
            //_logger.DebugFormatEx("Configured schema for profobuf serialization infrastructure : {0}", configuredSchema);
        }

        public IEnumerable<BrokeredMessage> Convert(TrackedUseCase trackedUseCase)
        {
            var messages = new List<BrokeredMessage>();
            Stream stream = null;
            BrokeredMessage msg;

            try
            {
                stream = new MemoryStream();
                _protobufModel.Serialize(stream, trackedUseCase);
                stream.Position = 0;

                msg = new BrokeredMessage(stream, true);
                msg.Properties.Add(TrackedUseCaseMessageProperties.Indicator.Name, TrackedUseCaseMessageProperties.Indicator.Value);
                msg.Properties.Add(TrackedUseCaseMessageProperties.Names.MessageBodyType, (int)MessageBodyType.Binary);
                msg.Properties.Add(TrackedUseCaseMessageProperties.Names.FormatVersion, (int)TrackedUseCaseMessageFormatVersion.V1Entire);
                msg.Properties.Add(TrackedUseCaseMessageProperties.Names.Operation, trackedUseCase.RootNode.OperationIdentity.OperationIdentity.Id);
                msg.Properties.Add(TrackedUseCaseMessageProperties.Names.EntitiesSetHash, trackedUseCase.RootNode.OperationIdentity.Entities.EvaluateHash());
                msg.Properties.Add(TrackedUseCaseMessageProperties.Names.UseCaseId, trackedUseCase.RootNode.ScopeId);
                messages.Add(msg);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(
                    ex, 
                    "Can't serialize tracked use case to brokered message. Use case description: {0}", 
                    trackedUseCase);

                if (stream != null)
                {
                    stream.Dispose();
                }

                throw;
            }

            return new[] { msg };
        }
    }
}