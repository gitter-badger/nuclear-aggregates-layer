﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationConsistencyVerifier : IOperationConsistencyVerifier
    {
        private delegate bool OperationContextConsistencyChecker(IVerifierContext context, out bool isConsistent);

        private readonly ICommonLog _logger;
        private readonly OperationContextConsistencyChecker[] _checkers;

        public OperationConsistencyVerifier(ICommonLog logger)
        {
            _logger = logger;
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
                IsDeclaredChangesInconsistent(
                                              checkerContext.ComparableContext.AddedChanges,
                                              checkerContext.OperationScopesHierarchy,
                                              changesContext => changesContext.AddedChanges,
                                              ChangesType.Added,
                                              inconsistencyReport,
                                              severalModificationsReport);
            var deletedInconsistency =
                IsDeclaredChangesInconsistent(
                                              checkerContext.ComparableContext.DeletedChanges,
                                              checkerContext.OperationScopesHierarchy,
                                              changesContext => changesContext.DeletedChanges,
                                              ChangesType.Deleted,
                                              inconsistencyReport,
                                              severalModificationsReport);
            var updatedInconsistency =
                IsDeclaredChangesInconsistent(
                                              checkerContext.ComparableContext.UpdatedChanges,
                                              checkerContext.OperationScopesHierarchy,
                                              changesContext => changesContext.UpdatedChanges,
                                              ChangesType.Updated,
                                              inconsistencyReport,
                                              severalModificationsReport);

            isConsistent = !(addedInconsistency || deletedInconsistency || updatedInconsistency);
            if (!isConsistent)
            {
                _logger.ErrorFormatEx("Inconsistent operation context detected. Report: " + inconsistencyReport);
            }

            if (severalModificationsReport.Length > 0)
            {
                _logger.InfoEx(severalModificationsReport.ToString());
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
            OperationScopeNode targetScopeContext,
            Func<EntityChangesContext, IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>> declaredChangesExtractor,
            ChangesType changesType,
            StringBuilder inconsistencyReport,
            StringBuilder severalModificationsReport)
        {
            bool hasInconsistency = false;
            foreach (var changeEntry in trackedChanges)
            {
                foreach (var changedEntityId in changeEntry.Value)
                {
                    if (IsDeclaredChangesInconsistentDeep(
                                                          changeEntry.Key,
                                                          changedEntityId.Key,
                                                          targetScopeContext,
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
            OperationScopeNode targetScopeContext,
            Func<EntityChangesContext, IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>> declaredChangesExtractor,
            ChangesType changesType,
            StringBuilder severalModificationsReport)
        {
            var targetScopeChanges = declaredChangesExtractor(targetScopeContext.ScopeChanges);

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

            return targetScopeContext.Childs != null
                   &&
                   targetScopeContext.Childs.All(
                                                 node =>
                                                 IsDeclaredChangesInconsistentDeep(entityType,
                                                                                   entityId,
                                                                                   node,
                                                                                   declaredChangesExtractor,
                                                                                   changesType,
                                                                                   severalModificationsReport));
        }
    }
}