using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing
{
    public sealed class MessageBatchProcessingContext
    {
        private readonly IMessage[] _originalMessages;
        private readonly MessageProcessingStage[] _targetStages;
        private readonly IReadOnlyDictionary<Guid, MessageProcessingContext> _messageProcessings;

        public MessageBatchProcessingContext(IMessage[] originalMessages, MessageProcessingStage[] targetStages)
        {
            _originalMessages = originalMessages;
            _targetStages = targetStages;

            var messageProcessings = new Dictionary<Guid, MessageProcessingContext>();
            for (int i = 0; i < originalMessages.Length; i++)
            {
                var currentMessage = originalMessages[i];
                messageProcessings.Add(currentMessage.Id, new MessageProcessingContext(currentMessage, i, targetStages));
            }

            _messageProcessings = messageProcessings;
        }

        public IMessage[] OriginalMessages
        {
            get { return _originalMessages; }
        }

        public MessageProcessingStage[] TargetStages
        {
            get { return _targetStages; }
        }

        public IReadOnlyDictionary<Guid, MessageProcessingContext> MessageProcessings
        {
            get { return _messageProcessings; }
        }
    }
}