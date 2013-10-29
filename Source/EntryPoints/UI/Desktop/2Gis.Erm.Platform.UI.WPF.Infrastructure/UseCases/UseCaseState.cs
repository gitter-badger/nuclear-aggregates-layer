using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class UseCaseState : IUseCaseState
    {
        private readonly object _stateSynch = new object();
        private readonly IExecutingProcessingsRegistry _processingsRegistry;
        private readonly Dictionary<Guid, IUseCaseNode> _nodesRegistry = new Dictionary<Guid, IUseCaseNode>();
        
        private IUseCaseNode _current;
        private IUseCaseNode _root;

        public UseCaseState(IExecutingProcessingsRegistry processingsRegistry)
        {
            _processingsRegistry = processingsRegistry;
        }

        public bool IsEmpty
        {
            get
            {
                lock (_stateSynch)
                {
                    return _root == null;
                }
            }
        }

        public IUseCaseNode Root
        {
            get
            {
                return _root;
            }
        }

        public IUseCaseNode Current
        {
            get
            {
                lock (_stateSynch)
                {
                    return _current;
                }
            }
        }

        public IUseCaseNode[] NodesSnapshot
        {
            get
            {
                lock (_stateSynch)
                {
                    return _nodesRegistry.Values.ToArray();
                }
            }
        }
        
        public IExecutingProcessingsRegistry ProcessingsRegistry
        {
            get
            {
                return _processingsRegistry;
            }
        }

        public bool TryMoveNext(object context)
        {
            return TryAddNode(null, context);
        }

        public bool TryMoveNext(Guid previousNodeId, object context)
        {
            return TryAddNode(previousNodeId, context);
        }

        public bool Rollback(Guid targetNodeId)
        {   
            lock (_stateSynch)
            {
                if (_current.Parent == null || _current.Id != targetNodeId)
                {
                    return false;
                }

                _nodesRegistry.Remove(_current.Id);
                _current.Parent.Remove(_current.Id);
                _current.Dispose();
                _current = _current.Parent;
            }

            return true;
        }

        private bool TryAddNode(Guid? previousNodeId, object context)
        {
            lock (_stateSynch)
            {
                IUseCaseNode previousNode;

                if (!previousNodeId.HasValue)
                {
                    previousNode = _current;
                }
                else
                {
                    if (!_nodesRegistry.TryGetValue(previousNodeId.Value, out previousNode))
                    {
                        return false;
                    }

                }

                IUseCaseNode newNode;
                if (previousNode == null)
                {
                    if (_root != null)
                    {
                        throw new InvalidOperationException("Inconsisstent use case state");

                    }

                    newNode = new UseCaseNode(null);
                    _root = newNode;
                }
                else
                {
                    newNode = new UseCaseNode(previousNode);
                    previousNode.Add(newNode);
                }

                _nodesRegistry.Add(newNode.Id, newNode);
                newNode.Context = context;
                _current = newNode;
            }

            return true;
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
                    if (Root != null)
                    {
                        Root.Dispose();
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