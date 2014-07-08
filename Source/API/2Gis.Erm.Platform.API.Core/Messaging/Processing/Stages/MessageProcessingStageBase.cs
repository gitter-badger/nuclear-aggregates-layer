using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages
{
    public abstract class MessageProcessingStageBase<TActorFactory, TActor, TInput> : IMessageProcessingStage
        where TActorFactory : class
        where TActor : class
    {
        protected readonly TActorFactory ActorFactory;
        protected readonly ICommonLog Logger;

        protected MessageProcessingStageBase(TActorFactory actorFactory, ICommonLog logger)
        {
            ActorFactory = actorFactory;
            Logger = logger;
        }

        public abstract MessageProcessingStage Stage { get; }

        BatchStageResult IMessageProcessingStage.Process(
            IMessageFlow messageFlow,
            MessageBatchProcessingContext batchProcessingContext,
            IEnumerable<Guid> targetMessageIds)
        {
            Logger.DebugFormatEx("Starting stage. Flow: {0}. Stage: {1}", messageFlow, Stage);

            var actorContext = CreateContext(messageFlow, batchProcessingContext, targetMessageIds);

            IEnumerable<TActor> actors;
            try
            {
                actors = CreateActors(actorContext);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Can't create actor for processing flow {0} executing stage {1}", actorContext.MessageFlow, Stage);
                Logger.ErrorEx(ex, msg);

                return new BatchStageResult(Stage) { Succeeded = false };
            }

            var result = Execute(actors, actorContext);
            SetStageResults(batchProcessingContext, result);

            Logger.DebugFormatEx("Finished stage. Flow: {0}. Stage: {1}", messageFlow, Stage);

            return result;
        }

        protected abstract TInput EvaluateInput(MessageProcessingStageActorContext<TInput> context);

        protected abstract IEnumerable<TActor> CreateActors(MessageProcessingStageActorContext<TInput> context);

        protected abstract IReadOnlyDictionary<Guid, MessageProcessingStageResult> ExecuteActor(TActor actor, MessageProcessingStageActorContext<TInput> context);

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
                    Logger.ErrorEx(ex, msg);

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

        private static void SetStageResults(MessageBatchProcessingContext messageBatchProcessingContext, BatchStageResult batchStageResults)
        {
            foreach (var stageResult in batchStageResults.Results)
            {
                var messageProcessingContext = messageBatchProcessingContext.MessageProcessings[stageResult.Key];
                messageProcessingContext.Results[messageProcessingContext.CurrentStageIndex] = stageResult.Value;
                ++messageProcessingContext.CurrentStageIndex;
            }
        }
        
        private MessageProcessingStageActorContext<TInput> CreateContext(
            IMessageFlow messageFlow,
            MessageBatchProcessingContext batchProcessingContext,
            IEnumerable<Guid> targetMessageIds)
        {
            var context = new MessageProcessingStageActorContext<TInput>
                {
                    MessageFlow = messageFlow,
                    TargetMessageProcessings = batchProcessingContext.MessageProcessings.Where(x => targetMessageIds.Contains(x.Key)).Select(x => x.Value).ToArray(),
                };

            context.Input = EvaluateInput(context);

            return context;
        }
    }
}