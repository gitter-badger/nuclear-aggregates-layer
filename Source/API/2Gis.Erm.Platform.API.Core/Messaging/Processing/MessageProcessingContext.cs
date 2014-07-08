using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing
{
    public sealed class MessageProcessingContext
    {
        private readonly IMessage _message;
        private readonly int _messageOrdinalIndex;
        private readonly MessageProcessingStageResult[] _results;

        public MessageProcessingContext(IMessage message, int messageOrdinalIndex, IEnumerable<MessageProcessingStage> targetStages)
        {
            _message = message;
            _messageOrdinalIndex = messageOrdinalIndex;
            CurrentStageIndex = 0;

            _results = 
                targetStages
                    .Select(stage => new MessageProcessingStageResult(stage))
                    .ToArray();
        }

        public IMessage OriginalMessage
        {
            get { return _message; }
        }

        public int CurrentStageIndex { get; set; }

        public MessageProcessingStageResult[] Results
        {
            get { return _results; }
        }

        public int MessageOrdinalIndex
        {
            get { return _messageOrdinalIndex; }
        }
    }
}