using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public static class MessageProcessingStageResultUtils
    {
        public static MessageProcessingStageResult EmptyResult(this MessageProcessingStage stage)
        {
            return new MessageProcessingStageResult(stage);
        }

        public static MessageProcessingStageResult WithReport(this MessageProcessingStageResult result, params string[] reports)
        {
            result.Reports.AddRange(reports);
            return result;
        }
        
        public static MessageProcessingStageResult WithReport(this MessageProcessingStageResult result, IEnumerable<string> reports)
        {
            result.Reports.AddRange(reports);
            return result;
        }

        public static MessageProcessingStageResult WithExceptions(this MessageProcessingStageResult result, params Exception[] exceptions)
        {
            result.Exceptions.AddRange(exceptions);
            return result;
        }

        public static MessageProcessingStageResult WithExceptions(this MessageProcessingStageResult result, IEnumerable<Exception> exceptions)
        {
            result.Exceptions.AddRange(exceptions);
            return result;
        }
        
        public static MessageProcessingStageResult WithOutput(this MessageProcessingStageResult result, params IMessage[] messages)
        {
            result.OutputMessages.AddRange(messages);
            return result;
        }

        public static MessageProcessingStageResult WithOutput(this MessageProcessingStageResult result, IEnumerable<IMessage> messages)
        {
             result.OutputMessages.AddRange(messages);
            return result;
        }

        public static MessageProcessingStageResult AsSucceeded(this MessageProcessingStageResult result)
        {
            result.Succeeded = true;
            return result;
        }

        public static MessageProcessingStageResult AsFailed(this MessageProcessingStageResult result)
        {
            result.Succeeded = false;
            return result;
        }
    }
}