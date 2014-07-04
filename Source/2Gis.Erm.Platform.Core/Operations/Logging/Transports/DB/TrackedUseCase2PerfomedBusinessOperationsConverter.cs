﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public sealed class TrackedUseCase2PerfomedBusinessOperationsConverter : ITrackedUseCase2PerfomedBusinessOperationsConverter
    {
        private readonly IIdentityProvider _identityProvider;

        public TrackedUseCase2PerfomedBusinessOperationsConverter(IIdentityProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public IEnumerable<PerformedBusinessOperation> Convert(TrackedUseCase trackedUseCase)
        {
            var resultOperations = new List<PerformedBusinessOperation>();

            var queue = new Queue<Tuple<OperationScopeNode, long?>>();
            queue.Enqueue(Tuple.Create(trackedUseCase.RootNode, (long?)null));

            while (queue.Count > 0)
            {
                var queueElement = queue.Dequeue();

                var operation = CreateOperationInstance(queueElement.Item1, trackedUseCase.RootNode.ScopeId, queueElement.Item2);

                resultOperations.Add(operation);

                foreach (var childScope in queueElement.Item1.Childs)
                {
                    queue.Enqueue(Tuple.Create(childScope, (long?)operation.Id));
                }
            }

            return resultOperations;
        }

        private static void SerializeOperationChanges(XContainer context, IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> changes, ChangesType changesType)
        {
            foreach (var change in changes)
            {
                var changedEntityName = (int)change.Key.AsEntityName();
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

        private PerformedBusinessOperation CreateOperationInstance(OperationScopeNode scopesHierarchy, Guid rootScopeId, long? parentOperationId)
        {
            var operation = new PerformedBusinessOperation
            {
                Operation = scopesHierarchy.OperationIdentity.OperationIdentity.Id,
                OperationEntities = ResolveOperationEntitiesDescription(scopesHierarchy.OperationIdentity),
                Descriptor = scopesHierarchy.OperationIdentity.Entities.EvaluateHash(),
                UseCaseId = rootScopeId,
                Context = SerializeOperationChanges(scopesHierarchy),
                Date = DateTime.UtcNow,
                Parent = parentOperationId
            };

            _identityProvider.SetFor(operation);

            return operation;
        }

        private string ResolveOperationEntitiesDescription(StrictOperationIdentity operationIdentity)
        {
            return operationIdentity.Entities.Contains(EntitySet.EmptySetIndicator) ? null : string.Join(";", operationIdentity.Entities.Cast<int>());
        }

        private string SerializeOperationChanges(OperationScopeNode declaredChanges)
        {
            var context = new XElement("context");

            SerializeOperationChanges(context, declaredChanges.ChangesContext.AddedChanges, ChangesType.Added);
            SerializeOperationChanges(context, declaredChanges.ChangesContext.UpdatedChanges, ChangesType.Updated);
            SerializeOperationChanges(context, declaredChanges.ChangesContext.DeletedChanges, ChangesType.Deleted);

            return context.ToString(SaveOptions.DisableFormatting);
        }
    }
}