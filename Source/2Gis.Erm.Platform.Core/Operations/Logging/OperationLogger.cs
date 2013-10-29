using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationLogger : IOperationLogger
    {
        private readonly IRepository<PerformedBusinessOperation> _repository;
        private readonly IIdentityProvider _identityProvider;

        public OperationLogger(IRepository<PerformedBusinessOperation> repository, IIdentityProvider identityProvider)
        {
            _repository = repository;
            _identityProvider = identityProvider;
        }

        public void Log(OperationScopeNode scopesHierarchy)
        {
            var queue = new Queue<Tuple<OperationScopeNode, long?>>();
            queue.Enqueue(Tuple.Create(scopesHierarchy, (long?)null));

            while (queue.Count > 0)
            {
                var queueElement = queue.Dequeue();

                var operation = CreateOperationInstance(queueElement.Item1, queueElement.Item2);
                if (operation == null)
                {
                    continue;
                }

                _repository.Add(operation);

                foreach (var childScope in queueElement.Item1.Childs)
                {
                    queue.Enqueue(Tuple.Create(childScope, (long?)operation.Id));
                }
            }

            _repository.Save();
        }

        private PerformedBusinessOperation CreateOperationInstance(OperationScopeNode scopesHierarchy, long? parentOperationId)
        {
            string operationChangesDescription;
            if (!TrySerializeOperationChanges(scopesHierarchy, out operationChangesDescription))
            {
                return null;
            }

            var operation = new PerformedBusinessOperation
            {
                Operation = scopesHierarchy.Scope.OperationIdentity.OperationIdentity.Id,
                Descriptor = EntityNameUtils.EvaluateEntitiesHash(scopesHierarchy.Scope.OperationIdentity.Entities),
                Context = operationChangesDescription,
                Date = DateTime.UtcNow,
                    Parent = parentOperationId
            };

            _identityProvider.SetFor(operation);

            return operation;
        }

        // FIXME {all, 24.09.2013}: в 1.1 нужно добавить поддержку логирования самого факта операции, т.е. когда scope есть, но изменения никакие не задекларированы, т.о. контекст пустой
        private bool TrySerializeOperationChanges(OperationScopeNode declaredChanges, out string operationChangesDescription)
        {
            var context = new XElement("context");

            var changed = SerializeOperationChanges(context, declaredChanges.ScopeChanges.AddedChanges, ChangesType.Added);
            var updated = SerializeOperationChanges(context, declaredChanges.ScopeChanges.UpdatedChanges, ChangesType.Updated);
            var deleted = SerializeOperationChanges(context, declaredChanges.ScopeChanges.DeletedChanges, ChangesType.Deleted);

            operationChangesDescription = context.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);

            return changed || updated || deleted;
        }

        private static bool SerializeOperationChanges(XContainer context, IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> changes, ChangesType changesType)
        {
            var changesExist = false;
            foreach (var change in changes)
            {
                var changedEntityName = (int)change.Key.AsEntityName();
                foreach (var id in change.Value)
                {
                    changesExist = true;
                    var entity = new XElement("entity",
                                              new XAttribute("change", (int)changesType),
                                              new XAttribute("type", changedEntityName), // Строковое представление было бы более читаемым, но не устойчивым к рефакторингу
                                              new XAttribute("id", id.Key));
                    context.Add(entity);
                }
            }

            return changesExist;
        }
    }
}
