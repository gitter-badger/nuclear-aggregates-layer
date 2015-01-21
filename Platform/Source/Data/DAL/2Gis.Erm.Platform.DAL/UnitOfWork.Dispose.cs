using System.Linq;

namespace DoubleGis.Erm.Platform.DAL
{
    public abstract partial class UnitOfWork
    {
        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный + для подклассов
        /// </summary>
        protected bool IsDisposed
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
        /// Внутренний dispose самого базового класса
        /// </summary>
        public void Dispose()
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                // сначала вызываем реализацию у потомков
                OnDispose();

                // теперь отрабатывает сам базовый класс
                lock (_domainContextRegistrarSynch)
                {
                    HostDomainContextsStorage directlyNestedDomainContexts;
                    if (_domainContextRegistrar.TryGetValue(_directlyNestedDomainContextsHostId, out directlyNestedDomainContexts))
                    {
                        directlyNestedDomainContexts.ReadonlyDomainContext.Dispose();
                        foreach (var domainContext in directlyNestedDomainContexts.ModifiableDomainContexts.Values)
                        {
                            domainContext.Dispose();
                        }

                        _domainContextRegistrar.Remove(_directlyNestedDomainContextsHostId);
                    }

                    if (_domainContextRegistrar.Count > 0)
                    {
                        {
                            // Логирование
                            var directlyNestedDomainContextRepresentation = directlyNestedDomainContexts != null
                                ? string.Join(", ", directlyNestedDomainContexts.ModifiableDomainContexts.Keys)
                                : string.Empty;

                            var domainContextTextRepresentation = string.Join("\n", _domainContextRegistrar.Values.Select(x => string.Join(", ", x.ModifiableDomainContexts.Keys)));

                            _logger.ErrorEx(string.Format("При завершении UoW, обнаружены неочищенные domaincontext от каких-то domain context host - скорее всего где-то не вызвали dispose у UoWScope\nDirectly nested DC: {0}\nRemaining DCs: \n{1}",
                                directlyNestedDomainContextRepresentation,
                                domainContextTextRepresentation));
                        }

                        foreach (var hostDomainContextsStorage in _domainContextRegistrar.Values)
                        {
                            hostDomainContextsStorage.ReadonlyDomainContext.Dispose();
                            foreach (var domainContext in hostDomainContextsStorage.ModifiableDomainContexts.Values)
                            {
                                domainContext.Dispose();
                            }
                        }

                        _domainContextRegistrar.Clear();
                    }
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Обработчик dispose для подклассов
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}