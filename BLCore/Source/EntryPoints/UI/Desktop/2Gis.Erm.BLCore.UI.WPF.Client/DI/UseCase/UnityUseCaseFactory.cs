using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase
{
    public sealed class UnityUseCaseFactory : IUseCaseFactory
    {
        private sealed class PipelineDescriptor
        {
            public IPropagatorBlock<MessageProcessingContext, MessageProcessingContext> First { get; set; }
            public IPropagatorBlock<MessageProcessingContext, MessageProcessingContext> Last { get; set; }
            public Predicate<MessageProcessingContext> CanStartPipeline { get; set; }
        }

        private readonly IUnityContainer _container;
        private readonly IUseCaseHandlersRegistry _handlersRegistry;
        private readonly ITracer _tracer;

        public UnityUseCaseFactory(IUnityContainer container, IUseCaseHandlersRegistry handlersRegistry, ITracer tracer)
        {
            _container = container;
            _handlersRegistry = handlersRegistry;
            _tracer = tracer;
        }

        public IUseCase Create()
        {
            var useCaseScopeContainer = _container.CreateChildContainer();
            var useCaseId = Guid.NewGuid();
            var sequentialProcessingNetwork = CreateSequentialProcessingNetwork();
            var freeProcessingNetwork = CreateFreeProcessingNetwork();
            var useCase = 
                new Platform.UI.WPF.Infrastructure.UseCases.UseCase(
                    useCaseId,
                    new DataflowUseCaseMessageProcessor(sequentialProcessingNetwork, freeProcessingNetwork, _handlersRegistry, _tracer), 
                    new ExecutingProcessingsRegistry(_tracer), 
                    useCaseScopeContainer);
            useCaseScopeContainer.RegisterType<IMessageSink, UseCaseMessageSink>(Lifetime.Singleton, new InjectionConstructor(typeof(IUseCaseManager), useCaseId));
            return useCase;
        }

        private ITargetBlock<MessageProcessingContext> CreateSequentialProcessingNetwork()
        {
            var messageQueue = new BufferBlock<MessageProcessingContext>();
            var summator = new JoinBlock<MessageProcessingContext, MessageProcessingContext>();
            var converter = new TransformBlock<Tuple<MessageProcessingContext, MessageProcessingContext>, MessageProcessingContext>(input => input.Item1);
            var messageDistributor = new BroadcastBlock<MessageProcessingContext>(context => context);

            foreach (var handlersBucket in _handlersRegistry.Handlers)
            {
                var beforePipeline = GetStartBlock();
                var pipeline = GetProcessingPipeline(handlersBucket);
                var afterPipeline = GetFinishBlock();

                messageDistributor.LinkTo(beforePipeline, pipeline.CanStartPipeline);
                beforePipeline.LinkTo(pipeline.First);
                pipeline.Last.LinkTo(afterPipeline);
                afterPipeline.LinkTo(summator.Target2);
            }

            messageQueue.LinkTo(summator.Target1);
            summator.LinkTo(converter);
            converter.LinkTo(messageDistributor);

            // обеспечиваем прохождение первого сообщения,т.к. ранее обработанных ещё нет
            summator.Target2.Post(null);

            return messageQueue;
        }

        private ITargetBlock<MessageProcessingContext> CreateFreeProcessingNetwork()
        {
            var inputBuffer = new BufferBlock<MessageProcessingContext>();
            var messageDistributor = new BroadcastBlock<MessageProcessingContext>(context => context);

            foreach (var handlersBucket in _handlersRegistry.Handlers)
            {
                var beforePipeline = GetStartBlock();
                var pipeline = GetProcessingPipeline(handlersBucket);
                var afterPipeline = GetFinishBlock();

                messageDistributor.LinkTo(beforePipeline, pipeline.CanStartPipeline);
                beforePipeline.LinkTo(pipeline.First);
                pipeline.Last.LinkTo(afterPipeline);
            }

            inputBuffer.LinkTo(messageDistributor);
            return inputBuffer;
        }

        private IPropagatorBlock<MessageProcessingContext, MessageProcessingContext> GetStartBlock()
        {
            return new TransformBlock<MessageProcessingContext, MessageProcessingContext>(
                            context =>
                                {
                                    context.UseCase.State.ProcessingsRegistry.StartProcessing(new ProcessingDescriptor<IMessage>(context.Message.Id, context.Message));
                                    return context;
                                });
        }

        private PipelineDescriptor GetProcessingPipeline(KeyValuePair<Type, Type[]> handlersBucket)
        {
            var handlers = new List<IUseCaseMessageHandler>();
            var pipelineUnits = new List<IPropagatorBlock<MessageProcessingContext, MessageProcessingContext>>();
            for (int index = 0; index < handlersBucket.Value.Length; index++)
            {
                var handlerType = handlersBucket.Value[index];
                var handler = (IUseCaseMessageHandler)_container.Resolve(handlerType);
                handlers.Add(handler);

                var pipelineUnit = new TransformBlock<MessageProcessingContext, MessageProcessingContext>(
                    context =>
                        {
                            if (context.ProcessingResult != null)
                            {
                                return context;
                            }

                            try
                            {
                                if (!handler.CanHandle(context.Message, context.UseCase))
                                {
                                    return context;
                                }
                            }
                            catch (Exception ex)
                            {
                                var msg = string.Format("Handler \"CanHandle\" exception caught. Handler:{0}. UseCase:{1}. Message:{2}", handlerType, context.UseCase.Id, context.Message);
                                _tracer.Error(ex, msg);
                                context.PipelineUnhandledExceptions.Add(handlerType, new AggregateException(msg, ex));
                                return context;
                            }

                            var asyncHandler = handler as IUseCaseAsyncMessageHandler;
                            if (asyncHandler != null)
                            {
                                try
                                {
                                    var asyncResult = asyncHandler.Handle(context.Message, context.UseCase);
                                    asyncResult.Wait();
                                    context.ProcessingResult = asyncResult.Result;
                                    return context;
                                }
                                catch (Exception ex)
                                {
                                    var msg = string.Format(
                                        "Async handler \"Handle\" exception caught. Handler:{0}. UseCase:{1}. Message:{2}", handlerType, context.UseCase.Id, context.Message);
                                    _tracer.Error(ex, msg);
                                    context.PipelineUnhandledExceptions.Add(handler.GetType(), new AggregateException(msg, ex));
                                    return context;
                                }
                            }

                            var syncHandler = handler as IUseCaseSyncMessageHandler;
                            if (syncHandler != null)
                            {
                                try
                                {
                                    context.ProcessingResult = syncHandler.Handle(context.Message, context.UseCase);
                                    return context;
                                }
                                catch (Exception ex)
                                {
                                    var msg = string.Format(
                                        "Sync handler \"Handle\" exception caught. Handler:{0}. UseCase:{1}. Message:{2}", handlerType, context.UseCase.Id, context.Message);
                                    _tracer.Error(ex, msg);
                                    context.PipelineUnhandledExceptions.Add(handler.GetType(), new AggregateException(msg, ex));
                                    return context;
                                }
                            }

                            var notSupportedException =
                                new NotSupportedException(
                                    string.Format(
                                        "Required interface for sync or async handler isn't implemented. Handler:{0}. UseCase:{1}. Message:{2}",
                                        handlerType,
                                        context.UseCase.Id,
                                        context.Message));
                            _tracer.Error(notSupportedException.Message);
                            context.PipelineUnhandledExceptions.Add(handler.GetType(), notSupportedException);
                            return context;
                        });

                if (pipelineUnits.Count > 0)
                {
                    pipelineUnits[pipelineUnits.Count - 1].LinkTo(pipelineUnit);
                }

                pipelineUnits.Add(pipelineUnit);
            }

            var canStartPipelineCondition = (Predicate<MessageProcessingContext>) (context => handlersBucket.Key.IsInstanceOfType(context.Message));

            return new PipelineDescriptor
                       {
                           First = pipelineUnits[0], 
                           Last = pipelineUnits[pipelineUnits.Count - 1], 
                           CanStartPipeline = canStartPipelineCondition
                       };
        }

        private IPropagatorBlock<MessageProcessingContext, MessageProcessingContext> GetFinishBlock()
        {
            return new TransformBlock<MessageProcessingContext, MessageProcessingContext>(
                            context =>
                            {
                                context.UseCase.State.ProcessingsRegistry.FinishProcessing(context.Message.Id);
                                context.CompletionSource.SetResult(context);
                                return context;
                            });
        }
    }
}