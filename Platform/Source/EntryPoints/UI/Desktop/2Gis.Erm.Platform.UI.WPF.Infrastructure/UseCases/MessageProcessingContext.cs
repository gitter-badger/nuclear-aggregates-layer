using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class MessageProcessingContext : IMessageProcessingContext
    {
        private readonly IUseCase _useCase;
        private readonly IMessage _message;
        private readonly Guid _id = Guid.NewGuid();
        private readonly TaskCompletionSource<MessageProcessingContext> _completionSource = new TaskCompletionSource<MessageProcessingContext>();
        private readonly IDictionary<Type, Exception> _pipelineUnhandledExceptions = new Dictionary<Type, Exception>();
        
        public MessageProcessingContext(IUseCase useCase, IMessage message)
        {
            _useCase = useCase;
            _message = message;
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public IUseCase UseCase
        {
            get
            {
                return _useCase;
            }
        }

        public IMessage Message
        {
            get
            {
                return _message;
            }
        }

        public TaskCompletionSource<MessageProcessingContext> CompletionSource
        {
            get
            {
                return _completionSource;
            }
        }

        public IMessageProcessingResult ProcessingResult { get; set; }

        public IDictionary<Type, Exception> PipelineUnhandledExceptions
        {
            get
            {
                return _pipelineUnhandledExceptions;
            }
        }
    }
}
