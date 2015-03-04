using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Stages
{
    public sealed class AggregatedResultsHandlerMessageProcessingStage : MessageProcessingStageBase<IMessageAggregatedProcessingResultsHandlerFactory, 
                                                                                                    IMessageAggregatedProcessingResultsHandler, 
                                                                                                    AggregatedResultsHandlerMessageProcessingStage.AggregatedResultsInput>
    {
        public AggregatedResultsHandlerMessageProcessingStage(IMessageAggregatedProcessingResultsHandlerFactory actorFactory,
                                                              ITracer tracer)
            : base(actorFactory, tracer)
        {
        }

        public override MessageProcessingStage Stage
        {
            get { return MessageProcessingStage.Handle; }
        }

        protected override AggregatedResultsInput EvaluateInput(MessageProcessingStageActorContext<AggregatedResultsInput> context)
        {
            var appropriateFlows = new HashSet<IMessageFlow>();
            var processingResultsMap = new Dictionary<Guid, List<IProcessingResultMessage>>();

            foreach (var targetMessageProcessing in context.TargetMessageProcessings)
            {
                List<IProcessingResultMessage> resultsBatch;
                if (!processingResultsMap.TryGetValue(targetMessageProcessing.OriginalMessage.Id, out resultsBatch))
                {
                    resultsBatch = new List<IProcessingResultMessage>();
                    processingResultsMap.Add(targetMessageProcessing.OriginalMessage.Id, resultsBatch);
                }

                MessageProcessingStageResult previousResult;
                if (!TryGetPreviousStageResults(targetMessageProcessing, out previousResult) 
                    || !previousResult.Succeeded 
                    || previousResult.OutputMessages == null)
                {
                    continue;
                }

                foreach (var previousResults in previousResult.OutputMessages.OfType<IProcessingResultMessage>())
                {
                    resultsBatch.Add(previousResults);
                    appropriateFlows.Add(previousResults.TargetFlow);
                }
            }

            return new AggregatedResultsInput
                {
                    AppropriateFlows = appropriateFlows,
                    ProcessingResultsMap = processingResultsMap
                };
        }

        protected override IEnumerable<IMessageAggregatedProcessingResultsHandler> CreateActors(MessageProcessingStageActorContext<AggregatedResultsInput> context)
        {
            return ActorFactory.Create(context.Input.AppropriateFlows);
        }

        protected override IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> ExecuteActor(
            IMessageAggregatedProcessingResultsHandler actor, 
            MessageProcessingStageActorContext<AggregatedResultsInput> context)
        {
            return actor.Handle(context.Input.ProcessingResultsMap);
        }

        public sealed class AggregatedResultsInput
        {
            public ISet<IMessageFlow> AppropriateFlows { get; set; }
            public IReadOnlyDictionary<Guid, List<IProcessingResultMessage>> ProcessingResultsMap { get; set; }
        }
    }
}