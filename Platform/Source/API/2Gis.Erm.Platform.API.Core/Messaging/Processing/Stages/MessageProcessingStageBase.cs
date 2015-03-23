using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public abstract class MessageProcessingStageBase<TActorFactory, TActor, TInput> : IMessageProcessingStage
        where TActorFactory : class
        where TActor : class
    {
        protected readonly TActorFactory ActorFactory;
        protected readonly ITracer Tracer;

        protected MessageProcessingStageBase(TActorFactory actorFactory, ITracer tracer)
        {
            ActorFactory = actorFactory;
            Tracer = tracer;
        }

        public abstract MessageProcessingStage Stage { get; }

        public bool TryProcess(
            IMessageFlow messageFlow,
            IEnumerable<MessageProcessingContext> targetMessageProcessingContexts, 
            out IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> stageResults)
        {
            stageResults = Enumerable.Empty<KeyValuePair<Guid, MessageProcessingStageResult>>();

            Tracer.DebugFormat("Starting stage. Flow: {0}. Stage: {1}", messageFlow, Stage);

            MessageProcessingStageActorContext<TInput> actorContext;

            try
            {
                actorContext = CreateContext(messageFlow, targetMessageProcessingContexts);
            }
            catch (Exception ex)
            {
                Tracer.ErrorFormat(ex, "Can't create actor context for processing flow {0} executing stage {1}", messageFlow, Stage);
                return false;
            }

            IEnumerable<TActor> actors;
            try
            {
                actors = CreateActors(actorContext);
            }
            catch (Exception ex)
            {
                Tracer.ErrorFormat(ex, "Can't create actor for processing flow {0} executing stage {1}", actorContext.MessageFlow, Stage);
                return false;
            }

            try
            {
                var result = Execute(actors, actorContext);
                stageResults = result.Results;
            }
            catch (Exception ex)
            {
                Tracer.ErrorFormat(ex, "Actors execution failed for processing flow {0} executing stage {1}", actorContext.MessageFlow, Stage);
                return false;
            }

            Tracer.DebugFormat("Finished stage. Flow: {0}. Stage: {1}", messageFlow, Stage);
            return true;
        }

        protected abstract TInput EvaluateInput(MessageProcessingStageActorContext<TInput> context);
        protected abstract IEnumerable<TActor> CreateActors(MessageProcessingStageActorContext<TInput> context);
        protected abstract IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> ExecuteActor(TActor actor, MessageProcessingStageActorContext<TInput> context);

        /// <summary>
        /// Базовая реализация обработки потока сообщений последовательным вызовом actors
        /// Подклассы могут переопределить этот метод реализовав специфический механизм обработки, например через TPL, PLINQ и т.п.
        /// </summary>
        // COMMENT {all, 30.06.2014}: в идеале нужно отделить код слияния результатов обработки каждого actor с резултьтатами всей stages, чтобы в переопределенные реализации этого метода не были copypaste в значительной степени, однако, пока переопределенных нет, оставлено как есть
        protected virtual BatchStageResult Execute(IEnumerable<TActor> actors, MessageProcessingStageActorContext<TInput> context)
        {
            var batchResult = new BatchStageResult(Stage);
            bool isFailed = false;

            foreach (var actor in actors)
            {
                try
                {
                    var results = ExecuteActor(actor, context);

                    foreach (var resultBucket in results)
                    {
                        MessageProcessingStageResult stageResult;
                        if (!batchResult.Results.TryGetValue(resultBucket.Key, out stageResult))
                        {
                            stageResult = Stage.EmptyResult().AsSucceeded();
                            batchResult.AttachResults(resultBucket.Key, stageResult);
                        }

                        stageResult.WithExceptions(resultBucket.Value.Exceptions)
                                   .WithReport(resultBucket.Value.Reports)
                                   .WithOutput(resultBucket.Value.OutputMessages);

                        if (stageResult.Succeeded && !resultBucket.Value.Succeeded)
                        {
                            stageResult.AsFailed();
                            isFailed = true;
                        }
                    }

                    if (isFailed)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Can't execute actor flow {0} executing stage {1}", context.MessageFlow, Stage);
                    Tracer.Error(ex, msg);

                    foreach (var messageProcessing in context.TargetMessageProcessings)
                    {
                        batchResult.AttachResults(
                            messageProcessing.OriginalMessage.Id,
                            Stage.EmptyResult()
                                 .WithExceptions(ex)
                                 .WithReport(msg)
                                 .AsFailed());
                    }

                    isFailed = true;

                    break;
                }
                finally
                {
                    // COMMENT {all, 24.06.2014}: при такой реализации на каждый вызов actor должен создаваться новый, чтобы не было проблем с повторным использованием disposed экземпляра
                    var disposableActor = actor as IDisposable;
                    if (disposableActor != null)
                    {
                        disposableActor.Dispose();
                    }
                }
            }

            batchResult.Succeeded = !isFailed;
            return batchResult;
        }

        protected bool TryGetPreviousStageResults(MessageProcessingContext messageProcessingContext, out MessageProcessingStageResult previousResult)
        {
            previousResult = null;

            if (messageProcessingContext.CurrentStageIndex == 0)
            {
                previousResult = MessageProcessingStage.Received.EmptyResult().WithOutput(messageProcessingContext.OriginalMessage);
                return true;
            }

            var previousResultIndex = messageProcessingContext.CurrentStageIndex - 1;
            if (previousResultIndex < 0)
            {
                return false;
            }

            previousResult = messageProcessingContext.Results[previousResultIndex];
            return true;
        }
        
        private MessageProcessingStageActorContext<TInput> CreateContext(
            IMessageFlow messageFlow,
            IEnumerable<MessageProcessingContext> targetMessageProcessingContexts)
        {
            var context = new MessageProcessingStageActorContext<TInput>
                {
                    MessageFlow = messageFlow,
                    TargetMessageProcessings = targetMessageProcessingContexts.ToArray(),
                };

            context.Input = EvaluateInput(context);

            return context;
        }
    }
}