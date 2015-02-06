using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.DB;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public sealed class TrackedUseCase2PerfomedBusinessOperationsConverter : ITrackedUseCase2PerfomedBusinessOperationsConverter
    {
        private readonly IIdentityProvider _identityProvider;

        public TrackedUseCase2PerfomedBusinessOperationsConverter(IIdentityProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public IEnumerable<PerformedBusinessOperation> Convert(TrackedUseCase useCase)
        {
            var resultOperations = new List<PerformedBusinessOperation>();

            var queue = new Queue<OperationNode2PerformedOperationMapping>();
            queue.Enqueue(new OperationNode2PerformedOperationMapping(useCase.RootNode));

            while (queue.Count > 0)
            {
                var currentMapping = queue.Dequeue();
                var performedOperation = CreatePerformedOperation(currentMapping.OperationNode, useCase.RootNode.ScopeId, currentMapping.ParentPerformedOperationId);
                resultOperations.Add(performedOperation);

                var nestedOperations = useCase.GetNestedOperations(currentMapping.OperationNode.ScopeId);
                foreach (var nestedOperation in nestedOperations)
                {
                    queue.Enqueue(new OperationNode2PerformedOperationMapping(nestedOperation, performedOperation.Id));
                }
            }

            return resultOperations;
        }

        private PerformedBusinessOperation CreatePerformedOperation(OperationScopeNode operationNode, Guid rootScopeId, long? parentOperationId)
        {
            var operation = new PerformedBusinessOperation
            {
                Operation = operationNode.OperationIdentity.OperationIdentity.Id,
                OperationEntities = ResolveOperationEntitiesDescription(operationNode.OperationIdentity),
                Descriptor = operationNode.OperationIdentity.Entities.EvaluateHash(),
                UseCaseId = rootScopeId,
                Context = SerializeOperationChanges(operationNode),
                Date = DateTime.UtcNow,
                Parent = parentOperationId
            };

            _identityProvider.SetFor(operation);

            return operation;
        }

        private string ResolveOperationEntitiesDescription(StrictOperationIdentity operationIdentity)
        {
            return operationIdentity.Entities.Contains(EntitySet.EmptySetIndicator) ? null : string.Join(";", operationIdentity.Entities.Select(x => x.Id));
        }

        private string SerializeOperationChanges(OperationScopeNode declaredChanges)
        {
            var context = new XElement("context");

            SerializeOperationChanges(context, declaredChanges.ChangesContext.AddedChanges, ChangesType.Added);
            SerializeOperationChanges(context, declaredChanges.ChangesContext.UpdatedChanges, ChangesType.Updated);
            SerializeOperationChanges(context, declaredChanges.ChangesContext.DeletedChanges, ChangesType.Deleted);

            return context.ToString(SaveOptions.DisableFormatting);
        }
        
        private void SerializeOperationChanges(XContainer context, IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> changes, ChangesType changesType)
        {
            foreach (var change in changes)
            {
                var changedEntityName = change.Key.AsEntityName().Id;
                foreach (var id in change.Value)
                {
                    var entity = new XElement("entity",
                                              new XAttribute("change", (int)changesType),
                                              new XAttribute("type", changedEntityName), // Строковое представление было бы более читаемым, но не устойчивым к рефакторингу
                                              new XAttribute("id", id.Key));
                    context.Add(entity);
                }
            }
        }

        private class OperationNode2PerformedOperationMapping
        {
            private readonly OperationScopeNode _operationNode;
            private readonly long? _parentPerformedOperationId;

            public OperationNode2PerformedOperationMapping(OperationScopeNode operationNode, long? parentPerformedOperationId = null)
            {
                _operationNode = operationNode;
                _parentPerformedOperationId = parentPerformedOperationId;
            }

            public OperationScopeNode OperationNode
            {
                get { return _operationNode; }
            }

            public long? ParentPerformedOperationId
            {
                get { return _parentPerformedOperationId; }
            }
        }
    }
}