using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Stages
{
    public sealed class ProcessingStrategiesMessageProcessingStage : MessageProcessingStageBase<IMessageProcessingStrategyFactory,
                                                                                                IMessageProcessingStrategy,
                                                                                                IReadOnlyDictionary<Guid, IMessage>>
    {
        private readonly IMessageFlowRegistry _messageFlowRegistry;

        public ProcessingStrategiesMessageProcessingStage(IMessageFlowRegistry messageFlowRegistry,
                                                          IMessageProcessingStrategyFactory actorFactory,
                                                          ICommonLog logger)
            : base(actorFactory, logger)
        {
            _messageFlowRegistry = messageFlowRegistry;
        }

        public override MessageProcessingStage Stage
        {
            get { return MessageProcessingStage.Processing; }
        }

        protected override IReadOnlyDictionary<Guid, IMessage> EvaluateInput(MessageProcessingStageActorContext<IReadOnlyDictionary<Guid, IMessage>> context)
        {
            var inputsMap = new Dictionary<Guid, IMessage>();

            foreach (var targetMessageProcessing in context.TargetMessageProcessings)
            {
                MessageProcessingStageResult previousResult;
                if (!TryGetPreviousStageResults(targetMessageProcessing, out previousResult))
                {
                    throw new InvalidOperationException("Can't evaluate input for the stage");
                }

                inputsMap.Add(targetMessageProcessing.OriginalMessage.Id, previousResult.OutputMessages.Single());
            }

            return inputsMap;
        }

        protected override IEnumerable<IMessageProcessingStrategy> CreateActors(MessageProcessingStageActorContext<IReadOnlyDictionary<Guid, IMessage>> context)
        {
            var childFlows = _messageFlowRegistry.GetChildFlows(context.MessageFlow).ToArray();
            return context.Input.SelectMany(i => childFlows.Select(f => ActorFactory.Create(f, i.Value)));
        }

        protected override IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> ExecuteActor(
            IMessageProcessingStrategy actor,
            MessageProcessingStageActorContext<IReadOnlyDictionary<Guid, IMessage>> context)
        {
            var results = new Dictionary<Guid, MessageProcessingStageResult>();
            foreach (var input in context.Input)
            {
                if (!actor.CanProcess(input.Value))
                {
                    string msg = string.Format("Can't process message from flow {0} by strategy {1}", context.MessageFlow, actor.GetType().Name);
                    Logger.Error(msg);

                    results.Add(input.Key,
                                Stage.EmptyResult()
                                     .WithReport(msg)
                                     .AsFailed());
                }

                var processingResultMessage = actor.Process(input.Value);

                results.Add(input.Key,
                            Stage.EmptyResult()
                                 .WithOutput(processingResultMessage)
                                 .AsSucceeded());
            }

            return results;
        }
    }
}