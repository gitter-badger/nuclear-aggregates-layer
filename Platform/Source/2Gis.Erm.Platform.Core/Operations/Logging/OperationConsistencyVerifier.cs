using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationConsistencyVerifier : IOperationConsistencyVerifier
    {
        private delegate bool OperationContextConsistencyChecker(IVerifierContext context, out bool isConsistent);

        private readonly ITracer _tracer;
        private readonly OperationContextConsistencyChecker[] _checkers;

        public OperationConsistencyVerifier(ITracer tracer)
        {
            _tracer = tracer;
            _checkers = new OperationContextConsistencyChecker[]
                {
                    OperationDetailsToPersistenceChangesChecker,
                    OperationDetailsToMetadataSecurityRequirementsChecker
                };
        }

        public bool IsOperationContextConsistent(IEnumerable<IVerifierContext> verifierContexts)
        {
            foreach (var verifierContext in verifierContexts)
            {
                foreach (var checker in _checkers)
                {
                    bool isConsistent;
                    if (checker(verifierContext, out isConsistent) && !isConsistent)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool OperationDetailsToPersistenceChangesChecker(IVerifierContext context, out bool isConsistent)
        {
            isConsistent = false;

            var checkerContext = context as VerifierContext<IPersistenceChangesRegistry>;
            if (checkerContext == null)
            {
                return false;
            }

            var inconsistencyReport = new StringBuilder();
            var severalModificationsReport = new StringBuilder();
            var addedInconsistency =
                IsDeclaredChangesInconsistent(checkerContext.ComparableContext.AddedChanges,
                                              checkerContext.UseCase,
                                              changesContext => changesContext.AddedChanges,
                                              ChangesType.Added,
                                              inconsistencyReport,
                                              severalModificationsReport);
            var deletedInconsistency =
                IsDeclaredChangesInconsistent(checkerContext.ComparableContext.DeletedChanges,
                                              checkerContext.UseCase,
                                              changesContext => changesContext.DeletedChanges,
                                              ChangesType.Deleted,
                                              inconsistencyReport,
                                              severalModificationsReport);
            var updatedInconsistency =
                IsDeclaredChangesInconsistent(checkerContext.ComparableContext.UpdatedChanges,
                                              checkerContext.UseCase,
                                              changesContext => changesContext.UpdatedChanges,
                                              ChangesType.Updated,
                                              inconsistencyReport,
                                              severalModificationsReport);

            isConsistent = !(addedInconsistency || deletedInconsistency || updatedInconsistency);
            if (!isConsistent)
            {
                _tracer.ErrorFormat("Inconsistent operation context detected. Report: " + inconsistencyReport);
            }

            if (severalModificationsReport.Length > 0)
            {
                _tracer.Info(severalModificationsReport.ToString());
            }

            return true;
        }

        private bool OperationDetailsToMetadataSecurityRequirementsChecker(IVerifierContext context, out bool isConsistent)
        {
            isConsistent = false;

            var checkerContext = context as VerifierContext<IOperationSecurityRegistryReader>;
            if (checkerContext == null)
            {
                return false;
            }

            // TODO {all, 05.08.2013}: после появления полноценного mapping операций на требуемые для них привилегии безопасности нужно реализовать проверку все ли сущности по которым были изменения упомянуты в настройках безопасности операций 
            isConsistent = true;
            return true;
        }

        private bool IsDeclaredChangesInconsistent(
            IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> trackedChanges,
            TrackedUseCase useCase,
            Func<EntityChangesContext, IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>> declaredChangesExtractor,
            ChangesType changesType,
            StringBuilder inconsistencyReport,
            StringBuilder severalModificationsReport)
        {
            OperationScopeNode operationNode;
            if (!useCase.TryGetRootOperation(out operationNode))
            {
                throw new InvalidOperationException("Can't find root operation node for specified us case " + useCase);
            }

            bool hasInconsistency = false;
            foreach (var changeEntry in trackedChanges)
            {
                foreach (var changedEntityId in changeEntry.Value)
                {
                    if (IsDeclaredChangesInconsistentDeep(changeEntry.Key,
                                                          changedEntityId.Key,
                                                          useCase,
                                                          operationNode,
                                                          declaredChangesExtractor,
                                                          changesType,
                                                          severalModificationsReport))
                    {
                        inconsistencyReport.AppendFormat("Declared changes for {0} entities doesn't contains records for entity type {1} with entity id {2}{3}",
                                                         changesType,
                                                         changeEntry.Key.Name,
                                                         changedEntityId,
                                                         Environment.NewLine);

                        hasInconsistency = true;
                    }
                }
            }

            return hasInconsistency;
        }

        private bool IsDeclaredChangesInconsistentDeep(
            Type entityType,
            long entityId,
            TrackedUseCase useCase,
            OperationScopeNode operationNode,
            Func<EntityChangesContext, IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>> declaredChangesExtractor,
            ChangesType changesType,
            StringBuilder severalModificationsReport)
        {
            var targetScopeChanges = declaredChangesExtractor(operationNode.ChangesContext);

            ConcurrentDictionary<long, int> changedEntities;
            if (targetScopeChanges.TryGetValue(entityType, out changedEntities)
                && changedEntities != null
                && changedEntities.ContainsKey(entityId))
            {
                if (changedEntities[entityId] > 1)
                {
                    severalModificationsReport.AppendFormat("Same entity {0} was modified {4} times. Entity id: {1}. Modification type: {2}{3}",
                                                            entityType,
                                                            entityId,
                                                            changesType,
                                                            Environment.NewLine,
                                                            changedEntities[entityId]);
                }

                return false;
            }

            var nestedOperations = useCase.GetNestedOperations(operationNode.ScopeId);
            return nestedOperations != null
                   &&
                   nestedOperations.All(node => IsDeclaredChangesInconsistentDeep(entityType,
                                                                                 entityId,
                                                                                 useCase,
                                                                                 node,
                                                                                 declaredChangesExtractor,
                                                                                 changesType,
                                                                                 severalModificationsReport));
        }
    }
}