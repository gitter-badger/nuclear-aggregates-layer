using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public sealed class BatchStageResult
    {
        private readonly MessageProcessingStage _stage;
        private readonly Dictionary<Guid, MessageProcessingStageResult> _results = new Dictionary<Guid, MessageProcessingStageResult>();

        public BatchStageResult(MessageProcessingStage stage)
        {
            _stage = stage;
        }

        public MessageProcessingStage Stage
        {
            get { return _stage; }
        }

        public IReadOnlyDictionary<Guid, MessageProcessingStageResult> Results
        {
            get { return _results; }
        }

        public bool Succeeded { get; set; }

        public BatchStageResult AttachResults(Guid messageId, MessageProcessingStageResult messageProcessingStageResult)
        {
            _results.Add(messageId, messageProcessingStageResult);
            return this;
        }
    }
}