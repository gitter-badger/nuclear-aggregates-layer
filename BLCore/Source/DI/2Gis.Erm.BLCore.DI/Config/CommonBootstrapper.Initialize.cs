using System;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model;

using Microsoft.Practices.Unity;

using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.DI.Unity.Config;
using NuClear.DI.Unity.Config.RegistrationResolvers;
using NuClear.Storage.Writings;

using Mapping = DoubleGis.Erm.Platform.DI.Common.Config.Mapping;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        public static void InitializeDIInfrastructure(this IUnityContainer unityContainer)
        {
            unityContainer.AttachQueryableContainerExtension()
                          .UseParameterResolvers(new ParameterResolver[]
                                                     {
                                                         OnSimplifiedModelConsumerReadModelDependencyResolver,
                                                         OnSimplifiedModelConsumerDependencyResolver,
                                                         OnPersistenceServiceDependencyResolver,
                                                         OnOperationServicesDependencyResolver,
                                                         OnCrosscuttingDependencyResolver,
                                                         OnDynamicEntitiesRepositoriesDependencyResolver
                                                     }
                                                     .Concat(AggregatesLayerParameterResolvers.Defaults)
                                                     .Concat(ParameterResolvers.Defaults));
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
