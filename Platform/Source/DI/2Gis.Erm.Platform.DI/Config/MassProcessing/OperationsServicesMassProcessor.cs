using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Core.Metadata;
using DoubleGis.Erm.Platform.Core.Operations;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing
{
    public sealed class OperationsServicesMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;
        private readonly string _mappingScope;

        private readonly Func<Type, EntitySet, IEnumerable<Type>, Type>[] _entitySpecificOperationConflictResolvers;
        private readonly Func<Type, IEnumerable<Type>, Type>[] _nonCoupledOperationConflictResolvers;
        private readonly List<Type> _operationsImplementations = new List<Type>();
        private readonly IDictionary<Type, IOperationIdentity> _baseOperations2IdentitiesMap = new Dictionary<Type, IOperationIdentity>();
        private readonly List<IOperationIdentity> _declaredOperationIdentities = new List<IOperationIdentity>();

        public OperationsServicesMassProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeManagerFactoryMethod, string mappingScope)
            : this(container,
                   lifetimeManagerFactoryMethod,
                   mappingScope,
                   new Func<Type, EntitySet, IEnumerable<Type>, Type>[0],
                   new Func<Type, IEnumerable<Type>, Type>[0])
        {
        }

        public OperationsServicesMassProcessor(IUnityContainer container,
                                               Func<LifetimeManager> lifetimeManagerFactoryMethod,
                                               string mappingScope,
                                               Func<Type, EntitySet, IEnumerable<Type>, Type>[] entitySpecificOperationConflictResolvers,
                                               Func<Type, IEnumerable<Type>, Type>[] nonCoupledOperationConflictResolvers)
        {
            if (entitySpecificOperationConflictResolvers == null)
            {
                throw new ArgumentNullException("entitySpecificOperationConflictResolvers");
            }

            if (nonCoupledOperationConflictResolvers == null)
            {
                throw new ArgumentNullException("nonCoupledOperationConflictResolvers");
            }

            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
            _mappingScope = mappingScope;
            _entitySpecificOperationConflictResolvers = entitySpecificOperationConflictResolvers;
            _nonCoupledOperationConflictResolvers = nonCoupledOperationConflictResolvers;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { OperationIndicators.Operation, OperationIndicators.OperationIdentity };
        }

        #region Implementation of ITypeRegistrationsMassProcessor

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types)
            {
                if (!type.IsInterface && !type.IsAbstract && OperationIndicators.OperationIdentity.IsAssignableFrom(type))
                {
                    _declaredOperationIdentities.Add((IOperationIdentity)Activator.CreateInstance(type));
                    continue;
                }

                if (type.IsClass && !type.IsAbstract)
                {
                    _operationsImplementations.Add(type);
                    continue;
                }

                if (!type.IsInterface
                    || OperationIndicators.Group.All.Contains(type)
                    || OperationIndicators.EntitySpecificOperation.IsAssignableFrom(type)
                    || !OperationIndicators.Operation.IsAssignableFrom(type))
                {
                    continue;
                }

                var identityType = GetIdentityType(type);
                var identity = (IOperationIdentity)Activator.CreateInstance(identityType);
                _baseOperations2IdentitiesMap.Add(type, identity);
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            ////  процесс состоит из этапов:
            ////  - обрабатываем все типы реализаций, выявляя реализуемые ими операции: какие операции, для каких типов сущностей и т.п.
            ////       На выходе получаем карту соотвествия реализаций и операций (возможно содержащую конфликты)
            ////  - обрабатываем найденные конфликты, используя указанные conflict resolvers. Конфликтом считается наличии больше допустимого числа (специфического для типа и свойств операции) реализаций для базовых операций.
            ////  - если остались не разрешенные конфликты бросаем exception с информацией о них и прекращаем обработку
            //// - конфликтов нет - выполняем регистрацию всего найденного богатства в DI контейнере

            var entitySpecificOperations2ImplementationsMap = new Dictionary<Type, Dictionary<EntitySet, HashSet<Type>>>();
            var notCoupledOperations2ImplementationsMap = new Dictionary<Type, HashSet<Type>>();

            foreach (var implementation in _operationsImplementations)
            {
                // получаем все реализованные данным классом интерфейсы операций, исключая маркерные интерфейсы инфраструктуры операций
                // интерфейс операции - широкое понятие и включает:
                //      1). интерфейсы базовых операции
                //      2). какие-то уточняющие интерфейсы, расширяющие какой-то интерфейс базовой операции в целях его уточнения, наложения constraint (через generic) и т.п.
                //      3). агрегирующие интерфейсы, расширяющие(объединяющие) несколько интерфейсов из вышеперечисленных двух категорий
                var implementedInterfacesWithoutOperationsMarkers =
                    implementation.GetInterfaces()
                                  .Where(t => OperationIndicators.Operation.IsAssignableFrom(t) &&
                                              !OperationIndicators.Group.All.Contains(t.IsGenericType ? t.GetGenericTypeDefinition() : t))
                                  .Distinct()
                                  .ToArray();

                // список реализованных базовых операций
                var implementedBasicOperations = implementedInterfacesWithoutOperationsMarkers.Where(IsBasicOperation).ToArray();

                // список всех интерфейсов, реализованных зависимым от типа сущности образом, в нем отсутствуют базовые операции, 
                // т.к. они по соглашению, никогда не расширяют сущностно специфичные маркерные интерфейсы инфраструктуры операций
                var entitySpecificOperations =
                    implementedInterfacesWithoutOperationsMarkers.Where(t => OperationIndicators.EntitySpecificOperation.IsAssignableFrom(t)).ToArray();
                
                // список базовых операций, которые реализованы зависимым от сущности образом (из самой базовой операции не следует её зависимоть от типа сущности,
                // определить это можно только проверив интерфейсы расширяющие интерфейс базовой операции)
                var entitySpecificallyImplementedBasicOperations = implementedBasicOperations.Where(t => entitySpecificOperations.Any(t.IsAssignableFrom)).ToArray();
                
                // список nonCoupled операций - как базовых, так и агрегирующих их интерфейсов (если они есть)
                var nonCoupledOperations = implementedInterfacesWithoutOperationsMarkers
                    .Where(t => !entitySpecificallyImplementedBasicOperations.Any(specificBasic => specificBasic.IsAssignableFrom(t)))
                    .ToArray();

                // проверяем соблюдение соглашения - в одном классе реализации нельзя смешивать реализацию сущностно специфичных операций с noncoupled операциями
                if (entitySpecificOperations.Length > 0 && nonCoupledOperations.Length > 0)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Operations concrete implementation {0} implement entity specific and noncoupled operations simultaneously - convention violated",
                            implementation));
                }

                if (nonCoupledOperations.Length > 0)
                {
                    ProcessNonCoupledOperations(notCoupledOperations2ImplementationsMap, implementation, nonCoupledOperations);
                    continue;
                }

                ProcessEntitySpecificOperations(entitySpecificOperations2ImplementationsMap,
                                                implementation,
                                                entitySpecificallyImplementedBasicOperations,
                                                entitySpecificOperations);
            }

            ResolveConflictsForEntitySpecificOperations(entitySpecificOperations2ImplementationsMap);
            ResolveConflictsForNonCoupledOperations(notCoupledOperations2ImplementationsMap);

            Validate(entitySpecificOperations2ImplementationsMap, notCoupledOperations2ImplementationsMap);

            ConfigureOperationsInfrastructureForDi(_container, entitySpecificOperations2ImplementationsMap, notCoupledOperations2ImplementationsMap);
        }

        private static Type GetIdentityType(Type operationType)
        {
            var identifiedOperation = operationType.GetInterfaces()
                    .SingleOrDefault(t => !OperationIndicators.Group.NotEntitySpecific.Contains(t)
                                        && t.IsGenericType
                                        && t.GetGenericTypeDefinition() == OperationIndicators.IdentifiedOperation);
            if (identifiedOperation == null)
            {
                throw new InvalidOperationException(string.Format("Type {0} has marked with operation indicator is not marked with identified operation indicator {1}. ", operationType.FullName, OperationIndicators.IdentifiedOperation.FullName));
            }

            var identityType = identifiedOperation.GetGenericArguments().Single();
            if (!OperationIndicators.OperationIdentity.IsAssignableFrom(identityType) || identityType.IsInterface || identityType.IsAbstract)
            {
                throw new InvalidOperationException(string.Format("Type {0} specified as operation identity have to be not abstract implementation of {1}", identityType, OperationIndicators.Operation.FullName));
            }

            return identityType;
        }

        private static void Validate(
            Dictionary<Type, Dictionary<EntitySet, HashSet<Type>>> entitySpecificOperationsMap,
            Dictionary<Type, HashSet<Type>> nonCoupledOperationsMap)
        {
            var sb = new StringBuilder();

            foreach (var entitySpecificOperation in entitySpecificOperationsMap)
            {
                foreach (var multipleImplementations in entitySpecificOperation.Value.Where(i => i.Value.Count > 1))
                {
                    sb.AppendFormat("Multiple implementations founded for abstraction. Abstraction: {0}. Entities: {1}. Conflicting imlementations: {2}{3}",
                                    entitySpecificOperation.Key,
                                    multipleImplementations.Key,
                                    TypesToReport(multipleImplementations.Value),
                                    Environment.NewLine);
                }
            }

            foreach (var nonCoupledOperation in nonCoupledOperationsMap)
            {
                if (nonCoupledOperation.Value.Count > 1)
                {
                    sb.AppendFormat("Multiple implementations founded for abstraction. Abstraction: {0}. Conflicting imlementations: {1}{2}",
                                    nonCoupledOperation.Key,
                                    TypesToReport(nonCoupledOperation.Value),
                                    Environment.NewLine);
                }

                if (entitySpecificOperationsMap.ContainsKey(nonCoupledOperation.Key))
                {   
                    // проверяем чтобы одна и та же операция не использовалась разными реализациями, где-то как not coupled, а где-то как entity specific
                    sb.AppendFormat("Same abstraction used as entity specific and as not coupled operation. Abstraction type{0}{1}",
                                    nonCoupledOperation.Key,
                                    Environment.NewLine);
                }
            }

            if (sb.Length > 0)
            {
                throw new InvalidOperationException("Conventions violation founded. " + sb);
            }
        }

        private static string TypesToReport(IEnumerable<Type> types)
        {
            var sb = new StringBuilder();
            foreach (var type in types)
            {
                sb.AppendFormat("{0};{1}", type, Environment.NewLine);
            }

            return sb.ToString();
        }

        private void ProcessNonCoupledOperations(Dictionary<Type, HashSet<Type>> nonCoupledOperationsMap,
                                                 Type operationConcreteImplementation,
                                                 IEnumerable<Type> processingNonCoupledOperations)
        {
            foreach (var nonCoupledOperation in processingNonCoupledOperations)
            {
                HashSet<Type> implementations;
                if (!nonCoupledOperationsMap.TryGetValue(nonCoupledOperation, out implementations))
                {
                    implementations = new HashSet<Type>();
                    nonCoupledOperationsMap.Add(nonCoupledOperation, implementations);
                }

                implementations.Add(operationConcreteImplementation);
            }
        }

        private void ProcessEntitySpecificOperations(Dictionary<Type, Dictionary<EntitySet, HashSet<Type>>> entitySpecificOperationsMap,
                                                     Type operationConcreteImplementation,
                                                     Type[] implementedEntitySpecificBasicOperations,
                                                     Type[] processingEntitySpecificOperations)
        {
            // проверяем все сущностно специфичные интерфейсы операций, (т.е. среди них нет интерфейсов базовых операций, маркерных интерфейсов инфраструктуры операций)
            // среди проверяемых могут быть и агрегирующие интерфейсы (расширяющие более чем 1 базовый)
            // Цель - выявить все расширяемые базовые операции конкретным проверяемым интерфейсом, а также соответсвие каждой базовой операции, типам сущностей
            foreach (var processingEntitySpecificOperation in processingEntitySpecificOperations)
            {
                var processingInterfaces = new List<Type>(processingEntitySpecificOperation.GetInterfaces()) { processingEntitySpecificOperation };

                var operationSpecificTypesList = new List<EntitySet>();

                // используя найденные интерфейсы базовых операций, расширяемые проверяемым интерфейсом
                foreach (var implementedEntitySpecificBasicOperation in implementedEntitySpecificBasicOperations)
                {
                    // находим для каждой проверяемой базовой операции сущностно специфичный маркерный интерфейс операции (generic по некоторому кол-ву типов)
                    var entitySpecificOperations =
                        processingInterfaces
                            .Where(t => implementedEntitySpecificBasicOperation.IsAssignableFrom(t) && t.IsGenericType)
                            .Select(t => t.GetInterfaces().Single(es => es.IsGenericType
                                                                        &&
                                                                        OperationIndicators.Group.EntitySpecificGeneric.Contains(es.GetGenericTypeDefinition())))
                            .ToArray();

                    foreach (var entitySpecificInterface in entitySpecificOperations)
                    {
                        // нужно выделить типы сущностей, относительно которых специфично реализована проверяемая базовая операция
                        var operationSpecificTypes = ResolveOperationSpecificEntities(entitySpecificInterface);
                        if (operationSpecificTypes == null)
                        {
                            throw new InvalidOperationException("Can't resolve entities types for entity specific operation. Operation: " +
                                                                entitySpecificInterface);
                        }

                        operationSpecificTypesList.Add(operationSpecificTypes);
                        AddEntitySpecificImplementationToMap(
                            entitySpecificOperationsMap,
                            implementedEntitySpecificBasicOperation,
                            operationSpecificTypes,
                            operationConcreteImplementation);
                    }
                }

                // обрабатываемый интерфейс - считается специфичным для всех типов сущностей, для которых специфичны расширяемые им базовые интерфейсы операций
                var mergedOperationSpecificTypes = operationSpecificTypesList.Merge();
                AddEntitySpecificImplementationToMap(entitySpecificOperationsMap,
                                                     processingEntitySpecificOperation,
                                                     mergedOperationSpecificTypes,
                                                     operationConcreteImplementation);
            }
        }

        private void AddEntitySpecificImplementationToMap(Dictionary<Type, Dictionary<EntitySet, HashSet<Type>>> entitySpecificOperationsMap,
                                                          Type implementedInterface,
                                                          EntitySet operationSpecificTypes,
                                                          Type operationConcreteImplementation)
        {
            Dictionary<EntitySet, HashSet<Type>> entitySpecificImplementations;
            if (!entitySpecificOperationsMap.TryGetValue(implementedInterface, out entitySpecificImplementations))
            {
                entitySpecificImplementations = new Dictionary<EntitySet, HashSet<Type>>();
                entitySpecificOperationsMap.Add(implementedInterface, entitySpecificImplementations);
            }

            HashSet<Type> implementations;
            if (!entitySpecificImplementations.TryGetValue(operationSpecificTypes, out implementations))
            {
                implementations = new HashSet<Type>();
                entitySpecificImplementations.Add(operationSpecificTypes, implementations);
            }

            implementations.Add(operationConcreteImplementation);
        }

        private EntitySet ResolveOperationSpecificEntities(Type processingType)
        {
            // проверяем как реализована операция - для какого-то конкретного типа сущности
            // или обобщенным образом - как open generic - это вариант по умолчанию
            if (!processingType.IsGenericType)
            {
                return null;
            }

            var targetEntityOperationType = OperationIndicators.Group.EntitySpecificGeneric.SingleOrDefault(t => t == processingType.GetGenericTypeDefinition());
            if (targetEntityOperationType == null)
            {
                return null;
            }

            var targetOperationEntitiesCount = targetEntityOperationType.GetGenericArguments().Length;
            var genericArguments = processingType.GetGenericArguments();
            if (genericArguments.Length != targetOperationEntitiesCount)
            {
                // TODO {all, 12.11.2013}: подумать корректна ли данная ситуация, пока выглядит как не корректная поэтому exception
                // хотя потенциально возможно, что у типа generic от трех параметров, а у в базовом интерфейсе сущностно специфичной операции, например, 2
                // при этом первые 2 соответствуют аргументам базового интерфейса сущностно специфичной операции, а
                // трейтий параметр, может использоваться для каких-то внутренний целей и т.п. типа реализатора
                throw new InvalidOperationException(
                    string.Format("Type {0} with {1} type arguments extend base interface for entity specific operation {2} with type args count {3}, type argumetns count mismatch",
                    processingType,
                    genericArguments.Length,
                    targetEntityOperationType,
                                  targetOperationEntitiesCount));
            }

            var entityNames = new IEntityType[targetOperationEntitiesCount];
            for (int i = 0; i < targetOperationEntitiesCount; i++)
            {
                var currentTypeParameter = genericArguments[i];
                var appropriateEntityName =
                    !currentTypeParameter.IsGenericParameter
                        ? currentTypeParameter.AsEntityName()
                        : EntitySet.OpenEntitiesSetIndicator;
                entityNames[i] = appropriateEntityName;
            }

            return entityNames.ToEntitySet();
        }

        private void ConfigureOperationsInfrastructureForDi(IUnityContainer container,
                                                            Dictionary<Type, Dictionary<EntitySet, HashSet<Type>>> entitySpecificOperationsMap,
                                                            Dictionary<Type, HashSet<Type>> nonCoupledOperationsMap)
        {
            foreach (var entitySpecificOperation in entitySpecificOperationsMap)
            {
                var useNamedScope = entitySpecificOperation.Value.Count > 1;
                foreach (var specificImplementations in entitySpecificOperation.Value)
                {
                    Type typeFrom = entitySpecificOperation.Key;
                    if (typeFrom.IsGenericType)
                    {
                        var genericArguments = typeFrom.GetGenericArguments();
                        if (!typeFrom.IsGenericTypeDefinition && genericArguments.Any(x => x.IsGenericParameter))
                        {
                            // это open generic
                            // open generics регистрируются как open generic abstraction => open generic implementation
                            typeFrom = typeFrom.GetGenericTypeDefinition();
                        }
                    }

                    container.RegisterTypeWithDependencies(typeFrom,
                                                           specificImplementations.Value.Single(),
                                                           useNamedScope ? specificImplementations.Key.ToString() : null,
                                                           _lifetimeManagerFactoryMethod(),
                                                           _mappingScope);
                }
            }

            foreach (var nonCoupledOperation in nonCoupledOperationsMap)
            {
                Type typeFrom = nonCoupledOperation.Key;
                if (typeFrom.IsGenericType)
                {
                    var genericArguments = typeFrom.GetGenericArguments();
                    if (!typeFrom.IsGenericTypeDefinition && genericArguments.Any(x => x.IsGenericParameter))
                    {
                        // это open generic
                        // open generics регистрируются как open generic abstraction => open generic implementation
                        typeFrom = typeFrom.GetGenericTypeDefinition();
                    }
                }

                container.RegisterTypeWithDependencies(typeFrom, nonCoupledOperation.Value.Single(), _lifetimeManagerFactoryMethod(), _mappingScope);
            }

            var entitySpecificOperationsMapShrink =
                entitySpecificOperationsMap.Where(t => IsBasicOperation(t.Key))
                                           .ToDictionary(t => t.Key, t => t.Value.ToDictionary(i => i.Key, i => i.Value.Single()));

            var nonCoupledOperationsMapShrink =
                nonCoupledOperationsMap.Where(t => IsBasicOperation(t.Key))
                                       .ToDictionary(t => t.Key, t => t.Value.Single());

            container
                .RegisterType<IOperationsRegistry, OperationsRegistry>(Lifetime.Singleton,
                                                                       new InjectionConstructor(entitySpecificOperationsMapShrink,
                                                                                                nonCoupledOperationsMapShrink,
                                                                                                _baseOperations2IdentitiesMap))
                .RegisterType<IOperationAcceptabilityRegistrar, OperationAcceptabilityRegistrar>(Lifetime.Singleton)
                .RegisterType<IOperationsMetadataProvider, OperationsMetadataProvider>(_lifetimeManagerFactoryMethod())

                // FIXME {d.ivanov, 02.12.2013}: IOperationsMetadataProvider зарегистрирован второй раз в _mappingScope, 
                //                               т.к. от него зависит операция IActionsHistoryService, а все зависимости операций регистрируются в _mappingScope.
                //                               Чтобы убрать эту двойную регистрацию, нужно избавиться от ErmScope и SecurityScope, т.к. в них сейчас уже нет смысла
                .RegisterType<IOperationsMetadataProvider, OperationsMetadataProvider>(_mappingScope, _lifetimeManagerFactoryMethod())
                
                .RegisterType<IOperationApplicabilityByMetadataResolver, OperationApplicabilityByMetadataResolver>(Lifetime.Singleton)
                .RegisterType<IOperationIdentityRegistry, OperationIdentityRegistry>(Lifetime.Singleton,
                                                                                     new InjectionConstructor(_declaredOperationIdentities));
        }

        private void ResolveConflictsForEntitySpecificOperations(Dictionary<Type, Dictionary<EntitySet, HashSet<Type>>> entitySpecificOperationsMap)
        {
            if (_entitySpecificOperationConflictResolvers == null || _entitySpecificOperationConflictResolvers.Length == 0)
            {
                return;
            }

            foreach (var entitySpecificOperation in entitySpecificOperationsMap)
            {
                foreach (var conflict in entitySpecificOperation.Value.Where(i => i.Value.Count > 1))
                {
                    foreach (var resolver in _entitySpecificOperationConflictResolvers)
                    {
                        var resolveResult = resolver(entitySpecificOperation.Key, conflict.Key, conflict.Value);
                        if (resolveResult != null)
                        {
                            conflict.Value.Clear();
                            conflict.Value.Add(resolveResult);
                            break;
                        }
                    }
                }
            }
        }

        private void ResolveConflictsForNonCoupledOperations(Dictionary<Type, HashSet<Type>> nonCoupledOperationsMap)
        {
            if (_nonCoupledOperationConflictResolvers == null || _nonCoupledOperationConflictResolvers.Length == 0)
            {
                return;
            }

            foreach (var conflict in nonCoupledOperationsMap.Where(i => i.Value.Count > 1))
            {
                foreach (var resolver in _nonCoupledOperationConflictResolvers)
                {
                    var resolveResult = resolver(conflict.Key, conflict.Value);
                    if (resolveResult != null)
                    {
                        conflict.Value.Clear();
                        conflict.Value.Add(resolveResult);
                        break;
                    }
                }
            }
        }

        public bool IsBasicOperation(Type checkingType)
        {
            return _baseOperations2IdentitiesMap.ContainsKey(checkingType);
        }

        #endregion
    }
}