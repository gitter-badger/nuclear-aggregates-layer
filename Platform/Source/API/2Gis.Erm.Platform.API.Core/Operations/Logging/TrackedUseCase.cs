using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class TrackedUseCase : MessageBase, IUseCaseTracker
    {
        private readonly object _operationsSync = new object();

        private readonly List<OperationScopeNode> _operations = new List<OperationScopeNode>();
        private readonly Dictionary<Guid, HashSet<Guid>> _operationsHierarchy = new Dictionary<Guid, HashSet<Guid>>();

        private readonly Dictionary<Guid, OperationScopeNode> _operationsMap = new Dictionary<Guid, OperationScopeNode>();
        private readonly Dictionary<StrictOperationIdentity, List<Guid>> _identities2OperationsMap = new Dictionary<StrictOperationIdentity, List<Guid>>();
        
        private Guid? _rootNodeId;
        private bool _changesCompleted;

        public override Guid Id
        {
            get
            {
                lock (_operationsSync)
                {
                    if (!_rootNodeId.HasValue)
                    {
                        throw new InvalidOperationException("Root node is not specified yet");
                    }

                    return _rootNodeId.Value;
                }
            }
        }

        public IUseCaseTracker Tracker
        {
            get { return this; }
        }

        public OperationScopeNode RootNode
        {
            get
            {
                lock (_operationsSync)
                {
                    if (!_rootNodeId.HasValue)
                    {
                        throw new InvalidOperationException("Root node is not specified yet");
                    }

                    return _operationsMap[_rootNodeId.Value];
                }
            }
        }

        public IReadOnlyCollection<OperationScopeNode> Operations
        {
            get
            {
                EnsureUseCaseCompleted();

                return _operations;
            }
        }

        public IReadOnlyDictionary<StrictOperationIdentity, OperationScopeNode[]> Identities2Operations
        {
            get
            {
                // COMMENT {all, 14.07.2014}: могут быть проблемы с performance, если в usecase будет много операций с разными operationidentity, если будут проблемы, возможно стоит просто расшарить внутреннюю коллекцию  _identities2OperationsMap
                EnsureUseCaseCompleted();

                return _identities2OperationsMap.ToDictionary(
                    pair => pair.Key, 
                    pair => pair.Value.Select(id => _operationsMap[id]).ToArray());
            }
        }

        public bool TryGetRootOperation(out OperationScopeNode operationScopeNode)
        {
            operationScopeNode = null;

            lock (_operationsSync)
            {
                if (!_rootNodeId.HasValue)
                {
                    return false;
                }

                return _operationsMap.TryGetValue(_rootNodeId.Value, out operationScopeNode);
            }
        }

        public bool TryGetOperation(Guid operationNodeId, out OperationScopeNode operationScopeNode)
        {
            lock (_operationsSync)
            {
                return _operationsMap.TryGetValue(operationNodeId, out operationScopeNode);
            }
        }

        public IReadOnlyCollection<OperationScopeNode> GetNestedOperations(Guid operationNodeId)
        {
            EnsureUseCaseCompleted();

            HashSet<Guid> nestedOperationNodes;
            return _operationsHierarchy.TryGetValue(operationNodeId, out nestedOperationNodes) 
                ? nestedOperationNodes.Select(id => _operationsMap[id]).ToArray() 
                : new OperationScopeNode[0];
        }

        public override string ToString()
        {
            lock (_operationsSync)
            {
                if (!_rootNodeId.HasValue)
                {
                    return "Empty use case";
                }

                const string Template = "Id={0}. Root operation: {1}";
                return string.Format(Template, _rootNodeId.Value, _operationsMap[_rootNodeId.Value].OperationIdentity);
            }
        }

        void IUseCaseTracker.AddOperation(OperationScopeNode operationScopeNode)
        {
            lock (_operationsSync)
            {
                EnsureUseCaseNotCompleted();

                if (_operationsMap.ContainsKey(operationScopeNode.ScopeId))
                {
                    throw new InvalidOperationException("Operation scope with id " + operationScopeNode.ScopeId + " is already attached to operations registry");
                }

                _operations.Add(operationScopeNode);
                _operationsMap.Add(operationScopeNode.ScopeId, operationScopeNode);

                List<Guid> concreteOperations;
                if (!_identities2OperationsMap.TryGetValue(operationScopeNode.OperationIdentity, out concreteOperations))
                {
                    concreteOperations = new List<Guid>();
                    _identities2OperationsMap.Add(operationScopeNode.OperationIdentity, concreteOperations);
                }

                concreteOperations.Add(operationScopeNode.ScopeId);

                if (operationScopeNode.IsRoot)
                {
                    _rootNodeId = operationScopeNode.ScopeId;
                }
            }
        }

        void IUseCaseTracker.AttachToParent(Guid parentOperationScopeId, params Guid[] childOperationScopeIds)
        {
            AttachToParent(parentOperationScopeId, childOperationScopeIds);
        }

        void IUseCaseTracker.AttachToParent(Guid parentOperationScopeId, IEnumerable<Guid> childOperationScopeIds)
        {
            AttachToParent(parentOperationScopeId, childOperationScopeIds);
        }

        void IUseCaseTracker.SynchronizeAuxiliaryData()
        {
            lock (_operationsSync)
            {
                EnsureUseCaseNotCompleted();

                _operationsMap.Clear();
                _identities2OperationsMap.Clear();

                foreach (var operation in _operations)
                {
                    _operationsMap.Add(operation.ScopeId, operation);

                    List<Guid> operationsList;
                    if (!_identities2OperationsMap.TryGetValue(operation.OperationIdentity, out operationsList))
                    {
                        operationsList = new List<Guid>();
                        _identities2OperationsMap.Add(operation.OperationIdentity, operationsList);
                    }

                    operationsList.Add(operation.ScopeId);
                }
            }
        }

        void IUseCaseTracker.Complete()
        {
            lock (_operationsSync)
            {
                _changesCompleted = true;
            }
        }

        private void EnsureUseCaseNotCompleted()
        {
            if (_changesCompleted)
            {
                throw new InvalidOperationException("Usecase is completed already. Operations can't be attached to usecase");
            }
        }

        private void EnsureUseCaseCompleted()
        {
            lock (_operationsSync)
            {
                if (!_changesCompleted)
                {
                    throw new InvalidOperationException("Usecase is not completed yet. Operations browsing available only after use case completion");
                }
            }
        }

        private void AttachToParent(Guid parentOperationScopeId, IEnumerable<Guid> childOperationScopeIds)
        {
            lock (_operationsSync)
            {
                EnsureUseCaseNotCompleted();

                HashSet<Guid> childList;
                if (!_operationsHierarchy.TryGetValue(parentOperationScopeId, out childList))
                {
                    childList = new HashSet<Guid>();
                    _operationsHierarchy.Add(parentOperationScopeId, childList);
                }

                foreach (var childOperationScopeId in childOperationScopeIds)
                {
                    childList.Add(childOperationScopeId);
                }
            }
        }
    }
}
