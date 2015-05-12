using System;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model;

using Microsoft.Practices.Unity;

using NuClear.Aggregates;
using NuClear.DI.Unity.Config;
using NuClear.DI.Unity.Config.RegistrationResolvers;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        public static void InitializeDIInfrastructure(this IUnityContainer unityContainer)
        {
            unityContainer.AttachQueryableContainerExtension()
                          .UseParameterResolvers(new ParameterResolver[]
                                                     {
                                                         OnAggregateReadModelDependencyResolver,
                                                         OnAggregateReadModelDependencyResolver,
                                                         OnAggregateRepositoryDependencyResolver,
                                                         OnSimplifiedModelConsumerReadModelDependencyResolver,
                                                         OnSimplifiedModelConsumerDependencyResolver,
                                                         OnPersistenceServiceDependencyResolver,
                                                         OnOperationServicesDependencyResolver,
                                                         OnCrosscuttingDependencyResolver,
                                                         OnDynamicEntitiesRepositoriesDependencyResolver
                                                     }
                                                     .Concat(ParameterResolvers.Defaults));
        }
        
        private static bool OnAggregateReadModelDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (constructorParameter.ParameterType.IsAggregateReadModel())
            {
                if (!constructorParameter.ParameterType.IsInterface)
                {
                    throw new InvalidOperationException("Aggregate read model can't be injected dependency as concrete type. Dependant consumer type: " + type + ". Dependency type:" + constructorParameter.ParameterType);
                }

                // constructorParameter - это read model
                // Тип для которго настраиваются зависимости, использует упрощенную работу с aggregates layer (через constructor injection и без явного использования UoW), 
                // и упростить её нужно здесь и сейчас
                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.ConstructorInjectionReadModelsScope);
                return true;
            }

            return false;
        }

        private static bool OnAggregateRepositoryDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (constructorParameter.ParameterType.IsAggregateRepository())
            {
                if (!constructorParameter.ParameterType.IsInterface)
                {
                    throw new InvalidOperationException("Aggregate repository can't be injected dependency as concrete type. Dependant consumer type: " + type + ". Dependency type:" + constructorParameter.ParameterType);
                }

                // Резолв aggregate service-а, который является зависимостью другого aggregate service-а
                if (type.IsAggregateRepository())
                {
                    resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.ConstructorInjectionNestedAggregateRepositoriesScope);
                    return true;
                }

                // constructorParameter - это аргегирующий репозиторий, 
                // Тип для которго настраиваются зависимости, использует упрощенную работу с aggregates layer (через constructor injection и без явного использования UoW), 
                // и упростить её нужно здесь и сейчас
                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.ConstructorInjectionAggregateRepositoriesScope);
                return true;
            }

            return false;
        }

        private static bool OnSimplifiedModelConsumerReadModelDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (constructorParameter.ParameterType.IsSimplifiedModelConsumerReadModel())
            {
                if (!constructorParameter.ParameterType.IsInterface)
                {
                    throw new InvalidOperationException("Simplified model consumer read model can't be injected dependency as concrete type. Dependant consumer type: " + type + ". Dependency type:" + constructorParameter.ParameterType);
                }

                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.SimplifiedModelConsumerReadModelScope);
                return true;
            }

            return false;
        }

        private static bool OnSimplifiedModelConsumerDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (constructorParameter.ParameterType.IsSimplifiedModelConsumer())
            {
                if (!constructorParameter.ParameterType.IsInterface)
                {
                    throw new InvalidOperationException("Simplified model consumer can't be injected dependency as concrete type. Dependant consumer type: " + type + ". Dependency type:" + constructorParameter.ParameterType);
                }

                // type - это потребитель использующий реализацию на базе Simplified ERM Domain Model
                // constructorParameter - это реализация использующая Simplified ERM Domain Model
                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.SimplifiedModelConsumerScope);
                return true;
            }

            return false;
        }

        private static bool OnPersistenceServiceDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (typeof(IPersistenceService).IsAssignableFrom(constructorParameter.ParameterType))
            {
                if (!constructorParameter.ParameterType.IsInterface)
                {
                    throw new InvalidOperationException("Persistence service can't be injected dependency as concrete type. Dependant consumer type: " + type + ". Dependency type:" + constructorParameter.ParameterType);
                }

                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.PersistenceServiceScope);
                return true;
            }

            return false;
        }

        private static bool OnOperationServicesDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (OperationIndicators.Operation.IsAssignableFrom(constructorParameter.ParameterType))
            {
                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType);
                return true;
            }

            return false;
        }

        private static bool OnCrosscuttingDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (!constructorParameter.ParameterType.IsCrosscuttingService())
            {
                return false;
            }

            if (!constructorParameter.ParameterType.IsCrosscuttingServiceInvariantSafe() && !type.IsCrosscuttingServiceConsumer())
            {
                var message = string.Format("Crosscutting service injection error. " +
                                            "Either service must be invariant-safe or service consumer must inherit from ICrosscuttingServiceConsumer. " +
                                            "Service type: {0}, consumer type: {1}",
                                            constructorParameter.ParameterType,
                                            type);
                throw new InvalidOperationException(message);
            }

            return false;
        }

        private static bool OnDynamicEntitiesRepositoriesDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            var repositoryMarker = typeof(IRepository<>);
            var dynamicEntityRepositoryMarker = typeof(IDynamicEntityRepository);
            resolvedParameter = null;

            if (constructorParameter.ParameterType.IsGenericType && constructorParameter.ParameterType.GetGenericTypeDefinition() == repositoryMarker && dynamicEntityRepositoryMarker.IsAssignableFrom(type))
            {
                resolvedParameter = new ResolvedParameter(constructorParameter.ParameterType, Mapping.DynamicEntitiesRepositoriesScope);
                return true;
            }

            return false;
        }
    }
}
