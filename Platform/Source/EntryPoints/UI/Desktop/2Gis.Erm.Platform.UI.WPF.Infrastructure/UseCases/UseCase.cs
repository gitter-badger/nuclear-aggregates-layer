using System;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class UseCase : IUseCase
    {
        private readonly Guid _id;
        private readonly IUseCaseMessageProcessor _processor;
        private readonly object _factoriesContext;
        private readonly IUseCaseState _state;

        public UseCase(Guid useCaseId, IUseCaseMessageProcessor processor, IExecutingProcessingsRegistry processingsRegistry, object factoriesContext)
        {
            _id = useCaseId;
            _state = new UseCaseState(processingsRegistry);
            _processor = processor;
            _factoriesContext = factoriesContext;
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public object FactoriesContext
        {
            get
            {
                return _factoriesContext;
            }
        }

        public IUseCaseState State
        {
            get
            {
                return _state;
            }
        }

        public MessageProcessingResult<TResult> Send<TResult>(IMessage message)
        {
            var processingContext = new MessageProcessingContext(this, message);
            return _processor.Send<TResult>(processingContext);
        }

        public bool Post(IMessage message)
        {
            var processingContext = new MessageProcessingContext(this, message);
            return _processor.Post(processingContext);
        }

        #region Поддержка IDisposable и Finalize

        private readonly object _disposeSync = new object();
        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    var disposableFactoriesContext = _factoriesContext as IDisposable;
                    if (disposableFactoriesContext != null)
                    {
                        disposableFactoriesContext.Dispose();
                    }
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }

        #endregion
    }
}