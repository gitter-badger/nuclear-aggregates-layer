using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public sealed class MessageProcessingStageResult
    {
        private readonly List<IMessage> _outputMessages  = new List<IMessage>();
        private readonly List<string> _reports = new List<string>();
        private readonly List<Exception> _exceptions = new List<Exception>();

        public MessageProcessingStageResult(MessageProcessingStage stage)
        {
            Stage = stage;
        }

        public MessageProcessingStage Stage { get; private set; }
        public bool Succeeded { get; set; }

        public List<IMessage> OutputMessages
        {
            get { return _outputMessages; }
        }

        public List<string> Reports 
        {
            get { return _reports; }
        }

        public List<Exception> Exceptions
        {
            get { return _exceptions; } 
        }
    }
}