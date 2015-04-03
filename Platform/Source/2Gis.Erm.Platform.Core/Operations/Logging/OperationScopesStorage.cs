using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationScopesStorage : IOperationScopeContextsStorage, IOperationScopeRegistrar
    {
        private readonly IFlowMarkerManager _flowMarkerManager;
        private readonly ConcurrentDictionary<Guid, FlowDescriptor> _flowsMap = new ConcurrentDictionary<Guid, FlowDescriptor>();

        private readonly TrackedUseCase _useCase;

        public OperationScopesStorage(IFlowMarkerManager flowMarkerManager)
        {
            _flowMarkerManager = flowMarkerManager;
            _useCase = new TrackedUseCase();
        }
        
        TrackedUseCase IOperationScopeContextsStorage.UseCase 
        {
            get { return _useCase; }
        }

        void IOperationScopeContextsStorage.Added<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities)
        {
            var targetEntityType = typeof(TEntity);
            if (targetEntityType.IsPersistenceOnly())
            {
                throw new InvalidOperationException(string.Format("Попытка добавления PersistenceOnly-сущности {0} в рамках бизнес-операции",
                                                                  targetEntityType.Name));
            }

            ResolveOperationNode(targetScope.Id).ChangesContext.Added<TEntity>(changedEntities);
        }

        void IOperationScopeContextsStorage.Deleted<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities)
        {
            var targetEntityType = typeof(TEntity);
            if (targetEntityType.IsPersistenceOnly())
            {
                throw new InvalidOperationException(string.Format("Попытка удаления PersistenceOnly-сущности {0} в рамках бизнес-операции",
                                                                  targetEntityType.Name));
            }

            ResolveOperationNode(targetScope.Id).ChangesContext.Deleted<TEntity>(changedEntities);
        }

        void IOperationScopeContextsStorage.Updated<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities)
        {
            var targetEntityType = typeof(TEntity);
            if (targetEntityType.IsPersistenceOnly())
            {
                throw new InvalidOperationException(string.Format("Попытка изменения PersistenceOnly-сущности {0} в рамках бизнес-операции",
                                                                  targetEntityType.Name));
            }

            ResolveOperationNode(targetScope.Id).ChangesContext.Updated<TEntity>(changedEntities);
        }

        void IOperationScopeRegistrar.RegisterRoot(IOperationScope rootScope)
        {
            OperationScopeNode rootScopeNode;
            if (_useCase.TryGetRootOperation(out rootScopeNode) && rootScopeNode != null)
            {
                throw new InvalidOperationException(
                    string.Format("Can't register scope with id {0} as root. Storage already contains root scope with id {1}. Possibly info about flow marker was lost because of thread switching and etc.", rootScope.Id, rootScopeNode.ScopeId));
            }

            if (!rootScope.IsRoot)
            {
                throw new InvalidOperationException(
                    string.Format("Can't register scope with id {0} as root. Scope internally declared is not root", rootScope.Id));
            }

            rootScopeNode = new OperationScopeNode(rootScope.Id, rootScope.IsRoot, rootScope.OperationIdentity);
            _useCase.Tracker.AddOperation(rootScopeNode);

            var flow = StartFlow();
            AttachScope2Flow(flow, rootScopeNode);
        }

        void IOperationScopeRegistrar.RegisterChild(IOperationScope childScope, IOperationScope parentScope)
        {
            FlowDescriptor currentFlow;
            if (TryGetCurrentFlow(out currentFlow))
            {
                // TODO {all, 05.08.2013}: подумать какая реакция будет более корректной в этой ситуации
                // пока действуем из предположения - когда явно указан parentscope, то сценарий многопоточный => считаем ошибкой если в многопоточном сценарии,
                // новый scope исполняется тем, же flow, что и какой-то из внешних, т.к. flowdescriptor в данной реализации использует stack, а при использовании одного и того же stack
                // несколькими потоками - вместо корректного знания о текущем scope, получим мешанину из всех scope разных потоков прицепленных к одному и тому же flow 
                // Один из вариантов решения - при явном указании parent scope всегда создавать новый flow с удалением marker, если он уже был (например, унаследован от родительского потока)
                // тут опасность - можно затирать marker и в однопоточном сценарии, если по ошибке явно указали parentscope - т.е. в этом варианте, надо запилить определение реально ли сценарий многопоточный или однопоточный
                // т.е. при однопоточном можно использовать flow parent scope, при многопоточном нужно создавать новый
                throw new InvalidOperationException("Can't register scope with explicit parent - flow already contains marker");
            }

            var childScopeNode = RegisterNewNestedOperationNode(childScope, parentScope.Id);

            currentFlow = StartFlow();
            AttachScope2Flow(currentFlow, childScopeNode);
        }

        void IOperationScopeRegistrar.RegisterAutoResolvedChild(IOperationScope childScope)
        {
            FlowDescriptor currentFlow;
            if (!TryGetCurrentFlow(out currentFlow))
            {
                throw new InvalidOperationException("Can't resolve parent scope for child - flow marker not found. Specify parent scope explicitly for multithreading scenarios");
            }

            Guid scopeId;
            if (!currentFlow.FlowOrder.TryPeek(out scopeId))
            {
                throw new InvalidOperationException("Can't resolve parent scope - can't get parent scope id from current flow");
            }

            var childScopeNode = RegisterNewNestedOperationNode(childScope, scopeId);
            AttachScope2Flow(currentFlow, childScopeNode);
        }

        void IOperationScopeRegistrar.Unregister(IOperationScope scope)
        {
            FlowDescriptor currentFlow;
            if (!TryGetCurrentFlow(out currentFlow))
            {
                throw new InvalidOperationException("Can't get current flow for specified scope " + scope.Id);
            }

            if (TryDeattachOperationNodeFromFlow(currentFlow, scope.Id))
            {
                CloseFlowIfEmpty(currentFlow);
            }

            var isRootScope = scope.Id == _useCase.RootNode.ScopeId;
            if (isRootScope)
            {
                if (!_flowsMap.IsEmpty)
                {
                    throw new InvalidOperationException("Can't close root scope with active flows in map. Check child scope exits for correctness.");
                }

                _useCase.Tracker.Complete();
            }
        }

        private void CloseFlowIfEmpty(FlowDescriptor flowDescriptor)
        {
            if (!flowDescriptor.FlowOrder.IsEmpty)
            {
                return;
            }

            FlowDescriptor removedDescriptor;
            if (!_flowsMap.TryRemove(flowDescriptor.Id, out removedDescriptor))
            {
                return;
            }

            removedDescriptor.FlowScopes.Clear();
            _flowMarkerManager.ClearMarker(removedDescriptor.Id);
        }

        private bool TryDeattachOperationNodeFromFlow(FlowDescriptor flowDescriptor, Guid targetOperationNodeId)
        {
            byte stubValue;
            if (!flowDescriptor.FlowScopes.TryGetValue(targetOperationNodeId, out stubValue))
            {
                return false;
            }

            Guid operationNodeId;
            if (flowDescriptor.FlowOrder.TryPeek(out operationNodeId)
                && operationNodeId == targetOperationNodeId
                && flowDescriptor.FlowOrder.TryPop(out operationNodeId))
            {
                if (operationNodeId != targetOperationNodeId)
                {
                    throw new InvalidOperationException(string.Format("Deattached operation node id:{0} != target operation node id:{1}", operationNodeId, targetOperationNodeId));
                }

                return true;
            }

            return false;
        }

        private OperationScopeNode ResolveOperationNode(Guid operationNodeId)
        {
            OperationScopeNode operationNode;
            if (!_useCase.TryGetOperation(operationNodeId, out operationNode))
            {
                throw new InvalidOperationException("Can't find registered operation scope for specified id: " + operationNodeId);
            }

            return operationNode;
        }

        private OperationScopeNode RegisterNewNestedOperationNode(IOperationScope operationScope, Guid parentOperationNodeId)
        {
            OperationScopeNode parentOperationNode;
            if (!_useCase.TryGetOperation(parentOperationNodeId, out parentOperationNode))
            {
                throw new InvalidOperationException("Can't find registered operation node for specified parent operation node id: " + parentOperationNodeId);
            }

            var operationNode = new OperationScopeNode(operationScope.Id, operationScope.IsRoot, operationScope.OperationIdentity);
            _useCase.Tracker.AddOperation(operationNode);
            _useCase.Tracker.AttachToParent(parentOperationNodeId, operationNode.ScopeId);
            return operationNode;
        }

        private bool TryGetCurrentFlow(out FlowDescriptor flowDescriptor)
        {
            flowDescriptor = null;

            Guid flowId;
            if (!_flowMarkerManager.TryGetMarker(out flowId))
            {
                return false;
            }

            return _flowsMap.TryGetValue(flowId, out flowDescriptor);
        }

        private FlowDescriptor StartFlow()
        {
            var flow = new FlowDescriptor(Guid.NewGuid());
            _flowMarkerManager.AddMarker(flow.Id);
            _flowsMap.TryAdd(flow.Id, flow);
            return flow;
        }

        private void AttachScope2Flow(FlowDescriptor flowDescriptor, OperationScopeNode operationScopeNode)
        {
            flowDescriptor.FlowOrder.Push(operationScopeNode.ScopeId);
            flowDescriptor.FlowScopes.TryAdd(operationScopeNode.ScopeId, 0);
        }

        private sealed class FlowDescriptor
        {
            private readonly Guid _flowId;

            // ConcurrentDictionary использован как суррогат для ConcurrentHashSet
            private readonly ConcurrentDictionary<Guid, byte> _flowScopes = new ConcurrentDictionary<Guid, byte>();
            private readonly ConcurrentStack<Guid> _flowOrder = new ConcurrentStack<Guid>();

            public FlowDescriptor(Guid flowId)
            {
                _flowId = flowId;
            }

            public Guid Id
            {
                get { return _flowId; }
            }

            public ConcurrentDictionary<Guid, byte> FlowScopes
            {
                get { return _flowScopes; }
            }

            public ConcurrentStack<Guid> FlowOrder
            {
                get { return _flowOrder; }
            }
        }
    }
}
