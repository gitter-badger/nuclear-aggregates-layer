using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public sealed class UseCaseNode : IUseCaseNode
    {
        private readonly ConcurrentDictionary<Guid, IUseCaseNode> _childs = new ConcurrentDictionary<Guid, IUseCaseNode>();
        private readonly IUseCaseNode _parent;

        private readonly Guid _id = Guid.NewGuid();

        public UseCaseNode(IUseCaseNode parent)
        {
            _parent = parent;
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public IUseCaseNode[] Childs 
        {
            get
            {
                return _childs.Values.ToArray();
            }
        }

        public IUseCaseNode Parent
        {
            get
            {
                return _parent;
            }
        }

        public bool IsDegenerated 
        {
            get
            {
                return !_childs.Any();
            }
        }

        public bool IsRoot 
        {
            get
            {
                return _parent == null;
            }
        }

        public object Context { get; set; }

        void IUseCaseNode.Add(IUseCaseNode child)
        {
            _childs.AddOrUpdate(child.Id, i => child,
                (id, node) =>
                    {
                        throw new InvalidOperationException(
                            "Child with specified id is already contained in childs list. ChildId: " + id);
                    });
        }

        bool IUseCaseNode.Remove(Guid childId)
        {
            IUseCaseNode removedNode;
            return _childs.TryRemove(childId, out removedNode);
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
                    // Free other state (managed objects).
                    var disposableContext = Context as IDisposable;
                    if (disposableContext != null)
                    {
                        disposableContext.Dispose();
                    }

                    // TODO пока циклы в графе не обработываем - подумать надо не надо и т.п.
                    Parallel.ForEach(_childs.Values, child => child.Dispose());
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
