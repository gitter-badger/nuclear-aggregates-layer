using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.DB
{
    public sealed class PerformedBusinessOperations2TrackedUseCaseTransformer<TMessageFlow> : 
        MessageTransformerBase<TMessageFlow, DBPerformedOperationsMessage, TrackedUseCase>
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly IOperationResolver _operationResolver;
        private readonly IOperationContextParser _operationContextParser;

        public PerformedBusinessOperations2TrackedUseCaseTransformer(
            IOperationResolver operationResolver,
            IOperationContextParser operationContextParser)
        {
            _operationResolver = operationResolver;
            _operationContextParser = operationContextParser;
        }

        protected override TrackedUseCase Transform(DBPerformedOperationsMessage originalMessage)
        {
            var operationNodesMap = new Dictionary<long, OperationScopeNode>();
            var operationHierarchyMap = new Dictionary<long, HashSet<long>>();

            var useCase = new TrackedUseCase();

            foreach (var operation in originalMessage.Operations)
            {
                var operationIdentity = _operationResolver.ResolveOperation(operation);
                EntityChangesContext changesContext;
                string report;
                if (!_operationContextParser.TryParse(operation.Context, out changesContext, out report))
                {
                    throw new InvalidOperationException("Can't parse operation context. OperationId=" + operation.Id + ". " + report);
                }

                OperationScopeNode operationNode;
                if (operation.Parent == null)
                {
                    operationNode = new OperationScopeNode(operation.UseCaseId, true, operationIdentity, changesContext);
                }
                else
                {
                    operationNode = new OperationScopeNode(Guid.NewGuid(), false, operationIdentity, changesContext);
                    
                    HashSet<long> childSet;
                    if (!operationHierarchyMap.TryGetValue(operation.Parent.Value, out childSet))
                    {
                        childSet = new HashSet<long>();
                        operationHierarchyMap.Add(operation.Parent.Value, childSet);
                    }

                    childSet.Add(operation.Id);
                }

                useCase.Tracker.AddOperation(operationNode);
                operationNodesMap.Add(operation.Id, operationNode);
            }
            
            var operationsHierarchy = new Dictionary<Guid, HashSet<Guid>>();
            foreach (var operationsBucket in operationHierarchyMap)
            {
                var parentOperation = operationNodesMap[operationsBucket.Key];
                
                HashSet<Guid> childsBucket;
                if (!operationsHierarchy.TryGetValue(parentOperation.ScopeId, out childsBucket))
                {
                    childsBucket = new HashSet<Guid>();
                    operationsHierarchy.Add(parentOperation.ScopeId, childsBucket);
                }

                if (operationsBucket.Value.Any())
                {
                    useCase.Tracker.AttachToParent(parentOperation.ScopeId, operationsBucket.Value.Select(id => operationNodesMap[id].ScopeId));
                }
            }

            useCase.Tracker.Complete();
            return useCase;
        }
    }
}
