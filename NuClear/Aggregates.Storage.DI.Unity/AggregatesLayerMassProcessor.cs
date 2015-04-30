using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace NuClear.Aggregates.Storage.DI.Unity
{
    public sealed class AggregatesLayerMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly Func<Type, string> _registrationTypeMappingResolver;

        private readonly List<Type> _aggregateRepositoryTypes = new List<Type>();
        private readonly List<Type> _readModelTypes = new List<Type>();

        public AggregatesLayerMassProcessor(IUnityContainer container, Func<Type, string> registrationTypeMappingResolver)
        {
            _container = container;
            _registrationTypeMappingResolver = registrationTypeMappingResolver;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { Indicators.Aggregates.ReadModel, Indicators.Aggregates.Repository };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                if (type.IsAggregateReadModel())
                {
                    _readModelTypes.Add(type);
                }

                if (type.IsAggregateRepository())
                {
                    _aggregateRepositoryTypes.Add(type);
                }
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            ProcessReadModels();
            ProcessAggregateRepositories();
        }

        private static bool ShouldBeProcessed(Type type)
        {
            if (type.GetTypeInfo().IsInterface || type.GetTypeInfo().IsAbstract)
            {
                return false;
            }

            return true;
        }

        private static object AggregateReadModelInjectionFactory(IUnityContainer container, Type readModelType, string registrationName)
        {
            var aggregateRepositoryFactory = container.Resolve<UnityAggregateReadModelFactory>();
            return aggregateRepositoryFactory.CreateAggregateReadModel(readModelType);
        }

        private static object AggregateRepositoryInjectionFactory(IUnityContainer container, Type repositoryType, string registrationName)
        {
            var aggregateRepositoryFactory = container.Resolve<UnityScopedAggregateRepositoryFactory>();
            return aggregateRepositoryFactory.CreateRepository(repositoryType);
        }
        
        private void ProcessAggregateRepositories()
        {
            var repositoriesMarkers = Indicators.Aggregates.Group.Repositories;
            
            foreach (var aggregateRepositoryType in _aggregateRepositoryTypes)
            {
                var implementedInterfacesWithoutMarkers =
                    aggregateRepositoryType.GetTypeInfo().ImplementedInterfaces
                                  .Where(t => t.IsAggregateRepository()
                                            && !repositoriesMarkers.Contains(t.GetTypeInfo().IsGenericType ? t.GetGenericTypeDefinition() : t))
                                  .Distinct()
                    .ToArray();

                // “.е. чтобы получить агрегирующий репозиторий нужно чтобы он реализовывал специфичный дл€ себ€ интерфейс (помимо IAggregateRepository) - 
                // именно такой интерфейс нужно запрашивать тогда, когда понадобилс€ конкретный агрегирующий репозиторий
                if (!implementedInterfacesWithoutMarkers.Any())
                {
                    throw new InvalidOperationException(string.Format("Aggregate repository {0} have to implement " +
                                                                      "contract interface and must be used through the interface",
                                                                      aggregateRepositoryType));
                }

                foreach (var aggregateRepositoryInterface in implementedInterfacesWithoutMarkers)
                {
                    var genericArguments = aggregateRepositoryInterface.GenericTypeArguments;
                    if (aggregateRepositoryInterface.GetTypeInfo().IsGenericType &&
                        !aggregateRepositoryInterface.GetTypeInfo().IsGenericTypeDefinition &&
                        genericArguments.Any(x => x.IsGenericParameter))
                    {
                        // –егистраци€ открытого generic-интерфейса и маппинг его на открытый generic-тип
                        // ¬ этом случае у типа T свойство IsGenericParameter = true
                        // Ќапример, IDeleteGenericEntityService<LegalPerson> <-> DeleteGenericEntityService<T>
                        // ѕричем, в зависимости от ограничений на T регистрируем тип в secure, либо unsecure scope; 
                        var genericTypeDefinition = aggregateRepositoryInterface.GetGenericTypeDefinition();
                        var mapping = _registrationTypeMappingResolver(aggregateRepositoryType);
                        
                        _container.RegisterType(genericTypeDefinition,
                                                aggregateRepositoryType,
                                                mapping,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));

                        // ѕоддержка возможности спросить открытый generic aggregate service в конструктор другого aggregate service
                        _container.RegisterType(genericTypeDefinition,
                                                aggregateRepositoryType,
                                                Mapping.ConstructorInjectionNestedAggregateRepositoriesScope,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));
                    }
                    else
                    {
                        // –егистраци€ закрытого generic-интерфейса и маппинг его на конретный тип
                        // ¬ этом случае у типа T = LegalPerson свойство IsGenericParameter = false, т.к. generic закрытый
                        // Ќапример, IDeleteGenericEntityService<LegalPerson> <-> DeleteLegalPersonService
                        // ¬ этом случае по€вл€етс€ возможность спросить в конструктор не полный агрегирующий репозиторий, а одну из его функцинальных частей, например, удаление
                        // ѕравильно это или нет, еще предстоит решить. ≈сли это не верно, сделать здесь ограничитель на такие кейсы
                        _container.RegisterType(aggregateRepositoryInterface,
                                                aggregateRepositoryType,
                                                Mapping.ConstructorInjectionAggregateRepositoriesScope,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));

                        // COMMENT {all, 19.09.2013}: –егистраци€ в дополнительном scope дл€ корректного резолвинга в UnityOperationServicesManager.GetEntitySpecificOperation
                        var mapping = _registrationTypeMappingResolver(aggregateRepositoryInterface);
                        _container.RegisterType(aggregateRepositoryInterface,
                                                aggregateRepositoryType,
                                                mapping,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(AggregateRepositoryInjectionFactory));

                        // регистрации, чтобы можно было €вно запрашивать создание агрегирующего репозитори€, 
                        // например у UoWScope, указыва€ интерфейс репозитори€, а не конкретный класс - реализацию
                        _container.RegisterType(aggregateRepositoryInterface,
                                                aggregateRepositoryType,
                                                Mapping.ExplicitlyCreatedAggregateRepositoriesScope,
                                                Lifetime.PerResolve);
                    }
                }

                _container.RegisterTypeWithDependencies(aggregateRepositoryType,
                                                        Mapping.ExplicitlyCreatedAggregateRepositoriesScope,
                                                        Lifetime.PerResolve,
                                                        null);
            }
        }

        private void ProcessReadModels()
        {
            foreach (var readModel in _readModelTypes)
            {
                var implementedInterfacesWithoutMarkers =
                    readModel.GetTypeInfo().ImplementedInterfaces
                             .Where(t => t.IsAggregateReadModel()
                                         && !Indicators.Aggregates.Group.ReadOnly.Contains(t.GetTypeInfo().IsGenericType ? t.GetGenericTypeDefinition() : t))
                             .Distinct()
                             .ToArray();

                // “.е. чтобы получить агрегирующий репозиторий нужно чтобы он реализовывал специфичный дл€ себ€ интерфейс (помимо IReadModel) - 
                // именно такой интерфейс нужно запрашивать тогда, когда понадобилс€ конкретный агрегирующий репозиторий
                if (!implementedInterfacesWithoutMarkers.Any())
                {
                    throw new InvalidOperationException("Read model " + readModel + " have to implement contract interface and must be used through the interface");
                }

                foreach (var readModelInterface in implementedInterfacesWithoutMarkers)
                {
                    var genericArguments = readModelInterface.GenericTypeArguments;
                    if (readModelInterface.GetTypeInfo().IsGenericType &&
                        !readModelInterface.GetTypeInfo().IsGenericTypeDefinition &&
                        genericArguments.Any(x => x.IsGenericParameter))
                    {
                        throw new InvalidOperationException("Open generic implementations for read model are not allowed");
                    }

                    _container.RegisterType(readModelInterface,
                                            readModel,
                                            Mapping.ConstructorInjectionReadModelsScope,
                                            Lifetime.PerResolve,
                                            new InjectionFactory(AggregateReadModelInjectionFactory));
                }
            }
        }
    }
}