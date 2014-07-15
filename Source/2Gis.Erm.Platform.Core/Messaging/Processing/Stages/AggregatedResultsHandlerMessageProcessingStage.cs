using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Stages
{
    public sealed class AggregatedResultsHandlerMessageProcessingStage :
        MessageProcessingStageBase<IMessageAggregatedProcessingResultsHandlerFactory, IMessageAggregatedProcessingResultsHandler, AggregatedResultsHandlerMessageProcessingStage.AggregatedResultsInput>
    {
        public AggregatedResultsHandlerMessageProcessingStage(
            IMessageAggregatedProcessingResultsHandlerFactory actorFactory, 
            ICommonLog logger) 
            : base(actorFactory, logger)
        {
        }

        public override MessageProcessingStage Stage
        {
            get { return MessageProcessingStage.Handle; }
        }

        protected override AggregatedResultsInput EvaluateInput(
            MessageProcessingStageActorContext<AggregatedResultsInput> context)
        {
            var input = new AggregatedResultsInput
                {
                    AppropriateFlows = new HashSet<IMessageFlow>(),
                    ProcessingResultsMap = new Dictionary<Guid, List<IProcessingResultMessage>>()
                };

            foreach (var targetMessageProcessing in context.TargetMessageProcessings)
            {
                MessageProcessingStageResult previousResult;
                if (!TryGetPreviousStageResults(targetMessageProcessing, out previousResult) || !previousResult.Succeeded)
                {
                    input.ProcessingResultsMap.Add(targetMessageProcessing.OriginalMessage.Id, null);
                    continue;
                }

                foreach (var previousResults in previousResult.OutputMessages.Cast<IProcessingResultMessage>())
                {
                    List<IProcessingResultMessage> resultsBatch;
                    if (!input.ProcessingResultsMap.TryGetValue(targetMessageProcessing.OriginalMessage.Id, out resultsBatch))
                    {
                        resultsBatch = new List<IProcessingResultMessage>();
                        input.ProcessingResultsMap.Add(targetMessageProcessing.OriginalMessage.Id, resultsBatch);
                    }

                    resultsBatch.Add(previousResults);
                    input.AppropriateFlows.Add(previousResults.TargetFlow);
                }
            }

            return input;
        }

        protected override IEnumerable<IMessageAggregatedProcessingResultsHandler> CreateActors(MessageProcessingStageActorContext<AggregatedResultsInput> context)
        {
            return ActorFactory.Create(context.Input.AppropriateFlows);
        }

        protected override IReadOnlyDictionary<Guid, MessageProcessingStageResult> ExecuteActor(
            IMessageAggregatedProcessingResultsHandler actor, 
            MessageProcessingStageActorContext<AggregatedResultsInput> context)
        {
            var results = new Dictionary<Guid, MessageProcessingStageResult>();

            var messageProcessingIds = new List<Guid>();
            var processings = new List<IProcessingResultMessage>();

            foreach (var processingResultsBucket in context.Input.ProcessingResultsMap)
            {
                if (processingResultsBucket.Value == null)
                {
                    results.Add(
                        processingResultsBucket.Key,
                        Stage.EmptyResult()
                             .WithReport("Can't evaluate processing results for aggregation. Orignial message id: " + processingResultsBucket.Key)
                             .AsFailed());
                    continue;
                }

                if (!actor.CanHandle(processingResultsBucket.Value))
                {
                    string msg = string.Format("Can't process message from flow {0} by strategy {1}", context.MessageFlow, actor.GetType().Name);
                    Logger.DebugEx(msg);

                    results.Add(processingResultsBucket.Key,
                                Stage.EmptyResult()
                                     .WithReport(msg)
                                     .AsFailed());
                    continue;
                }

                messageProcessingIds.Add(processingResultsBucket.Key);
                processings.AddRange(processingResultsBucket.Value);
            }

            var handledResults = actor.Handle(processings);

            foreach (var messageProcessingId in messageProcessingIds)
            {
                results.Add(
                    messageProcessingId, 
                    Stage.EmptyResult()
                         .AsSucceeded());
            }

            return results;
        }

        public sealed class AggregatedResultsInput
        {
            public ISet<IMessageFlow> AppropriateFlows { get; set; }
            public IDictionary<Guid, List<IProcessingResultMessage>> ProcessingResultsMap { get; set; }
        }
    }
}