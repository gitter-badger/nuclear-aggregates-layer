﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Ambivalent;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

using EntityNameUtils = DoubleGis.Erm.Platform.Model.Entities.EntityNameUtils;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation
{
    /// <summary>
    /// Проверяет разделение сущностей на соответствие соглашениям ERM о корректности имплементации Domain Model
    /// Т.е. сущности, основной модели, не могут быть в упрощенной, за исключением амбивалентных - и т.п.
    /// </summary>
    public sealed class CheckDomainModelEntitiesConsistencyMassProcessor : IMassProcessor
    {
        private static readonly Type ModelEntityIndicator = typeof(IEntity);
        private static readonly Type BaseEntityType = typeof(IBaseEntity);

        private readonly HashSet<Type> _modelEntityTypesIndex = new HashSet<Type>();

        public CheckDomainModelEntitiesConsistencyMassProcessor()
        {
            DoubleGis.Erm.Platform.Model.Entities.EntityTypeMap.Initialize();
        }

        #region Overrides of AbstractTypeRegistrationsMassProcessor

        public Type[] GetAssignableTypes()
        {
            return new[] { ModelEntityIndicator };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {   // выполняем проверки только при первом проходе
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                _modelEntityTypesIndex.Add(type);
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (!firstRun)
            {
                // процессинг при первом проходе
                return;
            }

            var violationsReport = new StringBuilder();

            EntityNameUniqueness(violationsReport);

            AutogeneratedEntitiesMustInModelEntitiesIndex(violationsReport);
            NonPersistanceOnlyMustHaveEntityName2TypeMapping(violationsReport);
            PersistanceOnlyEntitiesMustBePartOfDomainModel(violationsReport);
            PersistanceOnlyEntitiesCantHaveEntityName2TypeMapping(violationsReport);
            VirtualEntitiesCantHaveMappingOrPersistance(violationsReport);

            PersistenceOnlyEntitiesMustBeInModelEntitiesIndex(violationsReport);
            EntitiesHasMappingToTypesMustBeInModelEntitiesIndex(violationsReport);

            foreach (var entityName in EntityType.Instance.All().GetDecomposed())
            {
                // TODO {all, 26.07.2013}: Пока есть сущности прописанные как элементы разных агрегатов - нужно это исправить и включить проверку
                // BusinessModelEntityMustBePartOfSingleAggregate(entityName, ref violationsReport);
                EntityMustBePartOfBusinessOrSimplifiedModel(entityName, violationsReport);
                AmbivalentEntityMustBeElementOfBusinessAndSimplifiedModel(entityName, violationsReport);
                VirtualEntityCantBeElementOfBusinessOrSimplifiedModel(entityName, violationsReport);
                BusinessOrSimplifiedModelEntityMustHavePersistance(entityName, violationsReport);
            }

            if (violationsReport.Length != 0)
            {
                throw new InvalidOperationException("Domain model entities conventions violated. Report: " + violationsReport);
            }
        }

        private static bool ShouldBeProcessed(Type type)
        {
            // проверяем только реализации, причем не абстрактные
            if (type.IsInterface || !type.IsClass)
            {
                return false;
            }

            return true;
        }


        #endregion

        #region violations checkers

        private static void EntityNameUniqueness(StringBuilder report)
        {
            var entityNameType = typeof(EntityName);
            var entityNameMap = new Dictionary<EntityName, List<string>>();
            var allEntityNameEntries = Enum.GetNames(entityNameType);

            foreach (var entry in allEntityNameEntries)
            {
                var entryValue = (EntityName)Enum.Parse(entityNameType, entry);
                List<string> entriesList;
                if (!entityNameMap.TryGetValue(entryValue, out entriesList))
                {
                    entriesList = new List<string>();
                    entityNameMap.Add(entryValue, entriesList);
                }

                entriesList.Add(entry);
            }

            entityNameMap.Where(x => x.Value.Count > 1).Aggregate(report, (builder, pair) => builder.AppendFormat("Enum type {0} has entries {1} with the same value {2}{3}", entityNameType.Name, string.Join(", ", pair.Value), (int)pair.Key, Environment.NewLine));

            if (report.Length > 0)
            {
                throw new InvalidOperationException("Can't check domain model consistency. " + report);
            }
        }

        private static void PersistanceOnlyEntitiesCantHaveEntityName2TypeMapping(StringBuilder report)
        {
            foreach (var type in EntityNameUtils.PersistenceOnlyEntities)
            {
                IEntityType entityName;
                if (type.TryGetEntityName(out entityName))
                {
                    report.AppendFormat(
                        "Type {0} declared as persistance only and can't have mapping to {1}{2}",
                        type,
                        typeof(EntityName).Name,
                        Environment.NewLine);
                }
            }
        }

        private static void EntityMustBePartOfBusinessOrSimplifiedModel(IEntityType entityName, StringBuilder report)
        {
            if (entityName.IsVirtual())
            {
                return;
            }

            var aggregates = entityName.ToAggregates();
            if (aggregates.Any() && entityName.IsSimplifiedModel() && !entityName.IsAmbivalent())
            {
                report.AppendFormat(
                    "Not ambivalent entity {0} is declared as part of aggregate {1} and simplified model element {2}",
                    entityName,
                    string.Join(";", aggregates.Select(x => x.Description)),
                    Environment.NewLine);
            }
        }

        private static void AmbivalentEntityMustBeElementOfBusinessAndSimplifiedModel(IEntityType entityName, StringBuilder report)
        {
            if (entityName.IsVirtual())
            {
                return;
            }

            var aggregates = entityName.ToAggregates();
            if (entityName.IsAmbivalent() && !(aggregates.Any() && entityName.IsSimplifiedModel()))
            {
                report.AppendFormat(
                    "Ambivalent entity {0} must be element of business and simplified model simultaneously {1}",
                    entityName,
                    Environment.NewLine);
            }
        }

        private static void VirtualEntityCantBeElementOfBusinessOrSimplifiedModel(IEntityType entityName, StringBuilder report)
        {
            var aggregates = entityName.ToAggregates();
            if (entityName.IsVirtual() && (aggregates.Any() || entityName.IsSimplifiedModel()))
            {
                report.AppendFormat(
                    "Virtual entity {0} can't be element of business or simplified model {1}",
                    entityName,
                    Environment.NewLine);
            }
        }

        private void BusinessOrSimplifiedModelEntityMustHavePersistance(IEntityType entityName, StringBuilder report)
        {
            if (entityName.IsVirtual())
            {
                return;
            }

            var aggregates = entityName.ToAggregates();
            if (aggregates.Any() || entityName.IsSimplifiedModel())
            {
                Type entityType;
                if (!entityName.TryGetEntityType(out entityType))
                {
                    report.AppendFormat(
                        "Business or simplified model entity {0} must have persistance mapping {1}",
                        entityName,
                        Environment.NewLine);
                }
                else if (!_modelEntityTypesIndex.Contains(entityType))
                {
                    report.AppendFormat(
                        "Business or simplified model entity {0} have persistance mapping, but haven't domain entity type {1}",
                        entityName,
                        Environment.NewLine);
                }
            }
        }

        private void PersistenceOnlyEntitiesMustBeInModelEntitiesIndex(StringBuilder report)
        {
            foreach (var type in EntityNameUtils.PersistenceOnlyEntities)
            {
                if (!_modelEntityTypesIndex.Contains(type))
                {
                    report.AppendFormat(
                        "Type {0} is element of persistence only entities list, but not found in model entities index. Check implemented type interfaces{1}",
                        type,
                        Environment.NewLine);
                }
            }
        }

        private void EntitiesHasMappingToTypesMustBeInModelEntitiesIndex(StringBuilder report)
        {
            foreach (var mapping in NuClear.Model.Common.Entities.EntityTypeMap.EntitiesMapping)
            {
                if (!_modelEntityTypesIndex.Contains(mapping.Value) && !mapping.Value.IsBaseEntity())
                {
                    report.AppendFormat(
                        "Entity {0} has mapped to type {1} in entities types map, but not found in model entities index. Check implemented type interfaces{2}",
                        mapping.Key,
                        mapping.Value,
                        Environment.NewLine);
                }
            }
        }

        private void NonPersistanceOnlyMustHaveEntityName2TypeMapping(StringBuilder report)
        {
            foreach (var type in _modelEntityTypesIndex)
            {
                IEntityType entityName;
                if (!type.TryGetEntityName(out entityName) && !type.IsPersistenceOnly())
                {
                    report.AppendFormat(
                        "Type {0} is element of domain model, not persistance only, must have mapping {1}<->Entity Type {2}",
                        type,
                        typeof(EntityName).Name,
                        Environment.NewLine);
                }
            }
        }

        private static void PersistanceOnlyEntitiesMustBePartOfDomainModel(StringBuilder report)
        {
            foreach (var type in EntityNameUtils.PersistenceOnlyEntities)
            {
                if (!ModelEntityIndicator.IsAssignableFrom(type))
                {
                    report.AppendFormat(
                        "Type {0} declared as persistance only, have to be marked with {1}{2}",
                        type,
                        ModelEntityIndicator.Name,
                        Environment.NewLine);
                }
            }
        }

        private void AutogeneratedEntitiesMustInModelEntitiesIndex(StringBuilder report)
        {
            foreach (var type in ErmEntities.Entities)
            {
                if (!_modelEntityTypesIndex.Contains(type))
                {
                    report.AppendFormat(
                        "Auto generated type {0} is element of erm entities schema:\"ERM\", but not found in model entities index. Check implemented type interfaces{1}",
                        type,
                        Environment.NewLine);
                }
            }

            foreach (var type in ErmSecurityEntities.Entities)
            {
                if (!_modelEntityTypesIndex.Contains(type))
                {
                    report.AppendFormat(
                        "Auto generated type {0} is element of erm entities schema:\"SECURITY\", but not found in model entities index. Check implemented type interfaces{1}",
                        type,
                        Environment.NewLine);
                }
            }
        }

        private void VirtualEntitiesCantHaveMappingOrPersistance(StringBuilder report)
        {
            foreach (var entityName in EntityNameUtils.VirtualEntityNames)
            {
                Type entityType;
                if (entityName.TryGetEntityType(out entityType))
                {
                    var messageExtension = _modelEntityTypesIndex.Contains(entityType) ? ". Target mapped type is element of domain model." : string.Empty;
                    report.AppendFormat(
                        "{0} {1} is declared as virtual and can't have mapping to entity type {2}{3}{4}",
                        typeof(EntityName).Name,
                        entityName,
                        entityType,
                        messageExtension,
                        Environment.NewLine);
                }
            }
        }

        #endregion
    }
}
