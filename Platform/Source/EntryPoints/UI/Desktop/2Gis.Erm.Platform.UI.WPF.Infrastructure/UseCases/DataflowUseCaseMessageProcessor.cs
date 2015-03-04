using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;

using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class DataflowUseCaseMessageProcessor : IUseCaseMessageProcessor
    {
        private readonly ITargetBlock<MessageProcessingContext> _asyncInputBlock;
        private readonly ITargetBlock<MessageProcessingContext> _syncInputBlock;
        private readonly IUseCaseHandlersRegistry _handlersRegistry;
        private readonly ITracer _logger;

        public DataflowUseCaseMessageProcessor(
            ITargetBlock<MessageProcessingContext> asyncInputBlock, 
            ITargetBlock<MessageProcessingContext> syncInputBlock,
            IUseCaseHandlersRegistry handlersRegistry,
            ITracer logger)
        {
            _asyncInputBlock = asyncInputBlock;
            _syncInputBlock = syncInputBlock;
            _handlersRegistry = handlersRegistry;
            _logger = logger;
        }

        public MessageProcessingResult<TResult> Send<TResult>(MessageProcessingContext processingContext)
        {
            _logger.DebugFormat("Try send message. UseCase={0}. Message: {1}", processingContext.UseCase.Id, processingContext.Message);

            if (!IsProcessingContextValid(processingContext))
            {
                var msg = string.Format("Can't send. Unsupported message type. UseCase:{0}.Message:{1}", processingContext.UseCase.Id, processingContext.Message);
                _logger.Error(msg);
                throw new NotSupportedException(msg);
            }

            _syncInputBlock.Post(processingContext);
            processingContext.CompletionSource.Task.Wait();

            var result = processingContext.CompletionSource.Task.Result;
            if (result.PipelineUnhandledExceptions.Any())
            {
                throw new AggregateException(result.PipelineUnhandledExceptions.Values);
            }

            if (result == null)
            {
                throw new InvalidOperationException(string.Format(ResPlatformUI.MessageNotProcessedCorrectly, processingContext.Message.GetType().Name));
            }

            _logger.DebugFormat("Message sended. UseCase={0}. Message: {1}", processingContext.UseCase.Id, processingContext.Message);
            return (MessageProcessingResult<TResult>)processingContext.CompletionSource.Task.Result.ProcessingResult;
        }

        public bool Post(MessageProcessingContext processingContext)
        {
            bool postResult;
            _logger.DebugFormat("Try posting message. UseCase={0}. Message: {1}", processingContext.UseCase.Id, processingContext.Message);

            if (!IsProcessingContextValid(processingContext))
            {
                var msg = string.Format("Can't post. Unsupported message type. UseCase:{0}.Message:{1}", processingContext.UseCase.Id, processingContext.Message);
                _logger.Error(msg);
                throw new NotSupportedException(msg);
            }

            switch (processingContext.Message.ProcessingModel)
            {
                case ProcessingModel.Sequential:
                {
                    postResult = _asyncInputBlock.Post(processingContext);
                    break;
                }
                case ProcessingModel.Free:
                {
                    postResult = _syncInputBlock.Post(processingContext);
                    break;
                }
                default:
                {
                    throw new NotSupportedException("Unsupported message processing model. " + processingContext.Message.ProcessingModel);
                }
            }

            _logger.DebugFormat("Message posted. UseCase={0}. Message: {1}", processingContext.UseCase.Id, processingContext.Message);
            return postResult;
        }

        /// <summary>
        /// Проверяет возможна ли в принципе обработка сообщения, указанного типа (т.е. есть ли соответствующий handlers pipeline)
        /// Это важно, т.к. если pipeline нет, то в случае асинхронной обработки (через очередь), наличие на выходе очереди не поддерживаемого сообщения
        /// приведет к блокированию обработки всех последующих сообщений - т.к. косячное никогда не будет извлечено из очереди (пока не реализован какой-то вариант default pipeline)
        /// </summary>
        private bool IsProcessingContextValid(MessageProcessingContext processingContext)
        {
            var messageIndicator = processingContext.Message.GetType();
            Type[] appropriateHandlers;
            if (!_handlersRegistry.Handlers.TryGetValue(messageIndicator, out appropriateHandlers) || appropriateHandlers == null)
            {
                return false;
            }

            return true;
        }
    }
}