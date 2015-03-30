using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Stages
{
    public sealed class TransformMessageProcessingStage : MessageProcessingStageBase<IMessageTransformerFactory, IMessageTransformer, IReadOnlyDictionary<Guid, IMessage>>
    {
        public TransformMessageProcessingStage(IMessageTransformerFactory actorFactory, ITracer tracer) 
            : base(actorFactory, tracer)
        {
        }

        public override MessageProcessingStage Stage
        {
            get { return MessageProcessingStage.Transforming; }
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

        protected override IEnumerable<IMessageTransformer> CreateActors(MessageProcessingStageActorContext<IReadOnlyDictionary<Guid, IMessage>> context)
        {
            return context.Input.Select(i => ActorFactory.Create(context.MessageFlow, i.Value));
        }

        protected override IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> ExecuteActor(
            IMessageTransformer actor, 
            MessageProcessingStageActorContext<IReadOnlyDictionary<Guid, IMessage>> context)
        {
            var results = new Dictionary<Guid, MessageProcessingStageResult>();
            foreach (var input in context.Input)
            {
                if (!actor.CanTransform(input.Value))
                {
                    string msg = string.Format("Can't transform message from flow {0} by transformer {1}", context.MessageFlow, actor.GetType().Name);
                    Tracer.Error(msg);

                    results.Add(input.Key, 
                                Stage.EmptyResult()
                                     .WithReport(msg)
                                     .AsFailed());
                }

                var transformedMessage = actor.Transform(input.Value);

                results.Add(input.Key,
                            Stage.EmptyResult()
                                 .WithOutput(transformedMessage)
                                 .AsSucceeded());
            }

            return results;
        }
    }
}
