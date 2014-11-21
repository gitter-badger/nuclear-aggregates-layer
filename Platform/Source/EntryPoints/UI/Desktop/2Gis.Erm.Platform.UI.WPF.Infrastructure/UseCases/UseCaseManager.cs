using System;
using System.Collections.Concurrent;
using System.Linq;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Resolvers;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class UseCaseManager : IUseCaseManager, IMessageSink
    {
        private readonly IUseCaseResolversRegistry _resolversRegistry;
        private readonly IUseCaseFactory _useCaseFactory;

        private ConcurrentDictionary<Guid, IUseCase> _activeUseCases = new ConcurrentDictionary<Guid, IUseCase>();

        public UseCaseManager(IUseCaseResolversRegistry resolversRegistry, IUseCaseFactory useCaseFactory)
        {
            _resolversRegistry = resolversRegistry;
            _useCaseFactory = useCaseFactory;
        }

        MessageProcessingResult<TResult> IUseCaseManager.Send<TResult>(Guid useCaseToken, IMessage message)
        {   
            IUseCase useCase;
            if (!_activeUseCases.TryGetValue(useCaseToken, out useCase))
            {
                throw new InvalidOperationException("Invalid use case token " + useCaseToken);
            }

            return useCase.Send<TResult>(message);
        }

        bool IUseCaseManager.Post(Guid useCaseToken, IMessage message)
        {   
            IUseCase useCase;
            if (!_activeUseCases.TryGetValue(useCaseToken, out useCase))
            {
                throw new InvalidOperationException("Invalid use case token " + useCaseToken);
            }

            return useCase.Post(message);
        }

        MessageProcessingResult<TResult> IMessageSink.Send<TResult>(IMessage message)
        {   
            IUseCase targetUseCase;
            if (!TryResolveUseCase(message, out targetUseCase))
            {
                targetUseCase = _useCaseFactory.Create();
                _activeUseCases.TryAdd(targetUseCase.Id, targetUseCase);
            }

            return targetUseCase.Send<TResult>(message);
        }

        bool IMessageSink.Post(IMessage message)
        {   
            IUseCase targetUseCase;
            if (!TryResolveUseCase(message, out targetUseCase))
            {
                targetUseCase = _useCaseFactory.Create();
                _activeUseCases.TryAdd(targetUseCase.Id, targetUseCase);
            }

            return targetUseCase.Post(message);
        }

        private bool TryResolveUseCase(IMessage message, out IUseCase useCase)
        {
            useCase = null;

            var activeUseCases = _activeUseCases.Values.ToArray();

            IUseCaseResolver[] resolvers;
            if (_resolversRegistry.TryGetResolvers(message, out resolvers))
            {
                foreach (var checkingUseCase in activeUseCases)
                {
                    if (resolvers.Any(resolver => resolver.IsAppropriate(checkingUseCase, message)))
                    {
                        useCase = checkingUseCase;
                        return true;
                    }
                }
            }

            return false;
        }

        #region Поддержка IDisposable

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
                    // Free other state (managed objects).
                    var activeUseCases = _activeUseCases;
                    _activeUseCases = null;
                    if (activeUseCases != null)
                    {
                        foreach (var activeUseCase in activeUseCases)
                        {
                            activeUseCase.Value.Dispose();
                        }
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