using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Ambivalent;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing.Validation
{
    /// <summary>
    /// Проверяет реализацию Application Services на соответствие соглашениям ERM о корректности имплементации Domain Model
    /// </summary>
    public sealed class CheckApplicationServicesConventionsMassProcessor : IMassProcessor
    {
        private static readonly Type DataBaseCallerType = typeof(IDatabaseCaller);
        private static readonly Type SimplifiedPersistenceServiceType = typeof(ISimplifiedPersistenceService);

        private readonly HashSet<Type> _aggregateRepositories = new HashSet<Type>();
        private readonly HashSet<Type> _aggregateReadModels = new HashSet<Type>();
        private readonly HashSet<Type> _simplifiedModelConsumers = new HashSet<Type>();
        private readonly HashSet<Type> _simplifiedModelConsumerReadModels = new HashSet<Type>();

        public Type[] GetAssignableTypes()
        {
            return new[]
                {
                    ModelIndicators.Aggregates.Repository,
                    ModelIndicators.Aggregates.ReadModel,
                    ModelIndicators.Simplified.SimplifiedModelConsumer,
                    ModelIndicators.Simplified.SimplifiedModelConsumerReadModel
                };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {   // выполняем проверки только при первом проходе
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                if (type.IsAggregateRepository())
                {
                    _aggregateRepositories.Add(type);
                }

                if (type.IsAggregateReadModel())
                {
                    _aggregateReadModels.Add(type);
                }

                if (type.IsSimplifiedModelConsumer())
                {
                    _simplifiedModelConsumers.Add(type);
                }

                if (type.IsSimplifiedModelConsumerReadModel())
                {
                    _simplifiedModelConsumerReadModels.Add(type);
                }
            }
        }
        
        public void AfterProcessTypes(bool firstRun)
        {
            if (!firstRun)
            {
                // выполняем проверки только при первом проходе
                return;
            }

            foreach (var readModel in _aggregateReadModels)
            {
                AggregateReadModels.Validator.Validate(readModel);   
            }

            foreach (var aggregateRepository in _aggregateRepositories)
            {
                AggregateRepositories.Validator.Validate(aggregateRepository);
            }

            foreach (var simplifiedModelConsumer in _simplifiedModelConsumers)
            {
                SimplifiedModelConsumers.Validator.Validate(simplifiedModelConsumer);
            }

            foreach (var readModel in _simplifiedModelConsumerReadModels)
            {
                SimplifiedModelConsumerReadModels.Validator.Validate(readModel);
            } 
        }

        internal static string CheckSequenceAndInvalidElementsReport<TElement>(IEnumerable<TElement> sequence, Func<TElement, bool> isValidatePredicate)
        {
            var sb = new StringBuilder();
            bool hasInvalidElements = false;

            foreach (var element in sequence)
            {
                if (!isValidatePredicate(element))
                {
                    hasInvalidElements = true;
                    sb.Append(element + "; ");
                }
            }

            if (hasInvalidElements)
            {
                return sb.ToString();
            }

            return null;
        }

        private static bool ShouldBeProcessed(Type type)
        {
            // проверяем только реализации, причем не абстрактные
            if (type.IsInterface || type.IsAbstract)
            {
                return false;
            }

            return true;
        }

        private static IEnumerable<Type> UpdateableEntities(Type checkingType)
        {
            var constructor = checkingType.GetTargetConstructor();
            return constructor.GetParameters()
                .Where(p => p.ParameterType.IsGenericType
                        && !p.ParameterType.IsGenericTypeDefinition
                        && (p.ParameterType.GetGenericTypeDefinition() == typeof(IRepository<>)
                            || p.ParameterType.GetGenericTypeDefinition() == typeof(ISecureRepository<>)))
                .Select(p => p.ParameterType.GetGenericArguments()[0])
                .Distinct();
        }

        private class AggregateReadModels
        {
            private static readonly Lazy<AggregateReadModels> SingletonValidator = new Lazy<AggregateReadModels>(() => new AggregateReadModels());

            private AggregateReadModels()
            {
            }

            public static AggregateReadModels Validator
            {
                get { return SingletonValidator.Value; }
            }

            public void Validate(Type checkingType)
            {
                MustHaveValidAggregateRootSpecified(checkingType);
                ImplementationIsReadOnly(checkingType);
            }

            private void MustHaveValidAggregateRootSpecified(Type checkingType)
            {
                var aggregateRoot = ModelIndicators.AggregateForReadModel(checkingType);
                if (aggregateRoot == null)
                {
                    throw new InvalidOperationException(string.Format("Read model implementation {0} doesn't specifiy aggregate root", checkingType));
                }

                if (!AggregatesList.Aggregates.ContainsKey(aggregateRoot.AsEntityName()))
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of read model has specified aggregate root {1} that is not valid aggregate root. Check aggregates list", checkingType, aggregateRoot));
                }
            }

            private void ImplementationIsReadOnly(Type checkingType)
            {
                var invalidEntitiesReport =
                    CheckSequenceAndInvalidElementsReport(
                        UpdateableEntities(checkingType),
                        e => false);
                if (!string.IsNullOrEmpty(invalidEntitiesReport))
                {
                    throw new InvalidOperationException(
                        string.Format("Concrete implementation {0} of read model has ability to update entities. Invalid entities: {1}", checkingType, invalidEntitiesReport));
                }
            }
        }

        private class AggregateRepositories
        {
            private static readonly Lazy<AggregateRepositories> SingletonValidator = new Lazy<AggregateRepositories>(() => new AggregateRepositories());

            // TODO {all, 26.08.2013}: после рефакторинга слоя application services, в данном случае рапиливания мега агрегирующих репозиториев на операционно ориентированные части - можно будет удалить
            private readonly HashSet<Type> _nonSRPMonolithicAggregateRepositories = new HashSet<Type>();

            private AggregateRepositories()
            {
            }

            public static AggregateRepositories Validator
            {
                get
                {
                    return SingletonValidator.Value;
                }
            }

            public void Validate(Type checkingType)
            {
                var isOpenGeneric = checkingType.IsGenericType && checkingType.IsGenericTypeDefinition;
                if (isOpenGeneric)
                {
                    return;
                }

                SRPandNonSRPPatternsCantMixed(checkingType);
                SingleAggregateRootSpecified(checkingType);
                NestedAggregateServicesOnlyForPartableEntities(checkingType);

                // TODO {all, 27.08.2013}: временно отключена из-за массовых нарушений
                // AggregateRootIsUniqueAmongAggregates(checkingType); 
                EntitiesRepositoriesOnlyForAggregateEntities(checkingType);
                SimplifiedModelEntitiesIsReadOnly(checkingType);
                PersistenceServiceShouldOnlyWorkWithAggregateParts(checkingType);
                PersistenceServiceShouldBeUsedInsteadOfDatabaseCaller(checkingType);
                PersistenceServiceShouldBeUsedInsteadOfSimplified(checkingType);
            }

            /// <summary>
            /// Нельзя смешивать SRP (реализуют IAggregatePartRepository) и nonSRP (реализуют IAggregateRootRepository) подходы к реализации агрегирующих репозиториев. 
            /// Т.е. старые монолитные nonSRP агрегирующие репозиориии, должны оставаться как есть, по мере рефакторинга
            /// весь функционал должен переехать в SRP реализации операционноориентированых частей агрегирующих репозиториев
            /// </summary>
            private static void SRPandNonSRPPatternsCantMixed(Type checkingType)
            {
                var aggregateRootNonSRP = ModelIndicators.AggregateForRepositoryRoot(checkingType);
                var aggregateRootSRP = ModelIndicators.AggregateForRepositoryPart(checkingType);

                if (aggregateRootNonSRP != null && aggregateRootSRP != null)
                {
                    throw new InvalidOperationException(
                        string.Format("SRP (implement {0}) and nonSRP (implement {1}) patterns are mixed in type {2}",
                                      ModelIndicators.Aggregates.RepositoryPart,
                                      ModelIndicators.Aggregates.RepositoryRoot,
                                      checkingType.Name));
                }
            }

            /// <summary>
            /// Конкретная реализация агрегирующего репозитория должна иметь строго 1 указанный для неё корень агрегата (возможно указанный несколько раз) 
            /// </summary>
            private static void SingleAggregateRootSpecified(Type checkingType)
            {
                var aggregateRoot = ModelIndicators.AggregateForRepositoryRoot(checkingType);
                if (aggregateRoot == null)
                {   // видимо, данный тип, не является агегирующим репозиторием старого образца (до рефакторинга на SRP)
                    return;
                }

                if (!AggregatesList.Aggregates.ContainsKey(aggregateRoot.AsEntityName()))
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of aggregate repository has specified aggregate root {1} doesn't contains in aggregates list", checkingType, aggregateRoot));
                }
            }

            private static void NestedAggregateServicesOnlyForPartableEntities(Type checkingType)
            {
                var aggregateRepositoryDependencies = checkingType.GetTargetConstructor()
                                                                  .GetParameters()
                                                                  .Where(p => p.ParameterType.IsAggregateRepository())
                                                                  .Select(p => p.ParameterType)
                                                                  .ToArray();
                if (!aggregateRepositoryDependencies.Any())
                {
                    return;
                }

                var aggregateRoot = checkingType.ResolveAggregateRoot();
                var aggregateParts = checkingType.GetAggregateParts(aggregateRoot);
                if (!aggregateRoot.IsPartable() && !aggregateParts.Any(x => x.IsPartable()))
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of aggregate repository for aggregate root {1} " +
                                                                      "injects another aggregate repository, but neither the aggregate root or aggregate parts " +
                                                                      "is not partable entities",
                                                                      checkingType,
                                                                      aggregateRoot));
                }
            }

            /// <summary>
            /// Агрегирующий репозиторий может иметь в зависимостях сущностные репозитории только для сущностей своего агрегата.
            /// Т.е. проверяем что нет возможности изменять сущности не являющиеся частью данного агрегата, в обход соответсвующего им агрегирующего репозитория
            /// </summary>
            private static void EntitiesRepositoriesOnlyForAggregateEntities(Type checkingType)
            {
                var aggregateRoot = ModelIndicators.ResolveAggregateRoot(checkingType).AsEntityName();
                
                AggregateDescriptor aggregateDescriptor;
                if (!AggregatesList.Aggregates.TryGetValue(aggregateRoot, out aggregateDescriptor) || aggregateDescriptor == null)
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of aggregate repository has specified aggregate root {1} " +
                                                                      "without appropriate descriptor in aggregates list",
                                                                      checkingType,
                                                                      aggregateRoot));
                }

                var invalidEntitiesReport = CheckSequenceAndInvalidElementsReport(UpdateableEntities(checkingType),
                                                                                  e => e.IsPersistenceOnly() || aggregateDescriptor.AggregateEntities.Contains(e.AsEntityName()));
                if (!string.IsNullOrEmpty(invalidEntitiesReport))
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of aggregate repository has ability " +
                                                                      "to update entities not from appropriate aggregate. Invalid entities: {1}",
                                                                      checkingType,
                                                                      invalidEntitiesReport));
                }
            }

            /// <summary>
            /// Проверяем, что сущностей из упрощенной domain model нет среди сущностей, изменяемых данным агрегирующим репозиторием
            /// </summary>
            private static void SimplifiedModelEntitiesIsReadOnly(Type checkingType)
            {
                var invalidEntitiesReport =
                    CheckSequenceAndInvalidElementsReport(UpdateableEntities(checkingType), e => !(e.IsSimplifiedModel() && !e.IsAmbivalent()));
                if (!string.IsNullOrEmpty(invalidEntitiesReport))
                {
                    throw new InvalidOperationException(
                        string.Format("Aggregate repository: {0} use entity repository for simplified entity. Invalid entities: {1}", checkingType, invalidEntitiesReport));
                }
            }

            /// <summary>
            /// Проверяем, что в данный агрегирующий репозиторий инжектятся persistent services только для сущностей из своего аргерата
            /// </summary>
            private static void PersistenceServiceShouldOnlyWorkWithAggregateParts(Type checkingType)
            {
                var aggregateRoot = ModelIndicators.ResolveAggregateRoot(checkingType).AsEntityName();

                AggregateDescriptor aggregateDescriptor;
                if (!AggregatesList.Aggregates.TryGetValue(aggregateRoot, out aggregateDescriptor) || aggregateDescriptor == null)
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of aggregate repository has specified aggregate root {1} without appropriate descriptor in aggregates list", checkingType, aggregateRoot));
                }

                var persisteceServiceEntites = checkingType.GetTargetConstructor()
                                               .GetParameters()
                                               .Where(p => p.ParameterType.IsInterface && p.ParameterType.GetInterfaces().Any())
                                               .Select(p => p.ParameterType.GetInterfaces()[0])
                                               .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IPersistenceService<>))
                                               .Select(t => t.GetGenericArguments()[0])
                                               .ToArray();

                var invalidEntitiesReport =
                    CheckSequenceAndInvalidElementsReport(
                        persisteceServiceEntites,
                        e => aggregateDescriptor.AggregateEntities.Contains(e.AsEntityName()));

                if (!string.IsNullOrEmpty(invalidEntitiesReport))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Aggregate repository: {0} uses persistence services for entities those are not an aggregate part. Invalid entities: {1}",
                            checkingType,
                            invalidEntitiesReport));
                }
            }

            /// <summary>
            /// Проверяем, что данный агрегирующий репозиторий не использует IDataBaseCaller и ISimplifiedPersistenceService
            /// </summary>
            private static void PersistenceServiceShouldBeUsedInsteadOfDatabaseCaller(Type checkingType)
            {
                var databaseCallerUsed = checkingType.GetTargetConstructor()
                                                     .GetParameters()
                                                     .Any(p => DataBaseCallerType.IsAssignableFrom(p.ParameterType));

                if (databaseCallerUsed)
                {
                    throw new InvalidOperationException(string.Format("Aggregate repository: {0} uses IDatabaseCaller. Use entity-specific persistence service instead.",
                                                                      checkingType));
                }
            }

            /// <summary>
            /// Проверяем, что данный агрегирующий репозиторий не использует ISimplifiedPersistenceService
            /// </summary>
            private static void PersistenceServiceShouldBeUsedInsteadOfSimplified(Type checkingType)
            {
                var simplifiedServiceUsed = checkingType.GetTargetConstructor()
                                                     .GetParameters()
                                                     .Any(p => SimplifiedPersistenceServiceType.IsAssignableFrom(p.ParameterType));

                if (simplifiedServiceUsed)
                {
                    throw new InvalidOperationException(string.Format("Aggregate repository: {0} uses simplified persistence service. Use entity-specific persistence service instead.",
                                                                      checkingType));
                }
            }

            /// <summary>
            /// Указанный корень для агрегата должен быть уникальным среди реализаций агрегирующих репозиториев в старом nonSRP стиле
            /// </summary>
            private void AggregateRootIsUniqueAmongAggregates(Type checkingType)
            {
                var aggregateRoot = ModelIndicators.AggregateForRepositoryRoot(checkingType);
                if (aggregateRoot == null)
                {   // видимо, данный тип, не является агегирующим репозиторием старого образца (до рефакторинга на SRP)
                    return;
                }

                if (_nonSRPMonolithicAggregateRepositories.Contains(aggregateRoot))
                {
                    throw new InvalidOperationException("Entity type " + aggregateRoot + " specified as aggregate root more than once");
                }

                _nonSRPMonolithicAggregateRepositories.Add(aggregateRoot);
            }
            
        }

        private class SimplifiedModelConsumerReadModels
        {
            private static readonly Lazy<SimplifiedModelConsumerReadModels> SingletonValidator =
                new Lazy<SimplifiedModelConsumerReadModels>(() => new SimplifiedModelConsumerReadModels());

            private SimplifiedModelConsumerReadModels()
            {
            }

            public static SimplifiedModelConsumerReadModels Validator
            {
                get { return SingletonValidator.Value; }
            }

            public void Validate(Type checkingType)
            {
                ImplementationIsReadOnly(checkingType);
            }

            private void ImplementationIsReadOnly(Type checkingType)
            {
                var invalidEntitiesReport = CheckSequenceAndInvalidElementsReport(UpdateableEntities(checkingType),
                                                                                  e => false);
                if (!string.IsNullOrEmpty(invalidEntitiesReport))
                {
                    throw new InvalidOperationException(string.Format("Concrete implementation {0} of read model has ability to update entities. " +
                                                                      "Invalid entities: {1}",
                                                                      checkingType,
                                                                      invalidEntitiesReport));
                }
            }
        }
        
        private class SimplifiedModelConsumers
        {
            private static readonly Lazy<SimplifiedModelConsumers> SingletonValidator = new Lazy<SimplifiedModelConsumers>(() => new SimplifiedModelConsumers());
            
            private SimplifiedModelConsumers()
            {
            }

            public static SimplifiedModelConsumers Validator
            {
                get
                {
                    return SingletonValidator.Value;
                }
            }

            public void Validate(Type checkingType)
            {
                OnlySimplifiedModelEntitiesUpdatable(checkingType);
            }

            /// <summary>
            /// Проверяем, что потребитель упрощенной domain model обновляет только сущности из упрощенной Domain Model
            /// </summary>
            private void OnlySimplifiedModelEntitiesUpdatable(Type checkingType)
            {
                if (!checkingType.IsAbstract && checkingType.IsClass && checkingType.IsGenericTypeDefinition)
                {
                    return;
                }

                var invalidEntitiesReport = CheckSequenceAndInvalidElementsReport(UpdateableEntities(checkingType),
                                                                                  e => e.IsSimplifiedModel() || e.IsAmbivalent());
                if (!string.IsNullOrEmpty(invalidEntitiesReport))
                {
                    throw new InvalidOperationException(string.Format("Simplified model consumer: {0} use not only simplified entities. Invalid entities: {1}",
                                                                      checkingType,
                                                                      invalidEntitiesReport));
                }
            }
        }
    }
}