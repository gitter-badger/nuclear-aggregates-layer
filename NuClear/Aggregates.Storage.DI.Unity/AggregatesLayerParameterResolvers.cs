using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config.RegistrationResolvers;

namespace NuClear.Aggregates.Storage.DI.Unity
{
    public static class AggregatesLayerParameterResolvers
    {
        public static IEnumerable<ParameterResolver> Defaults
        {
            get
            {
                return new ParameterResolver[]
                    {
                        OnAggregateReadModelDependencyResolver,
                        OnAggregateRepositoryDependencyResolver
                    };
            }
        }

        private static bool OnAggregateReadModelDependencyResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter)
        {
            resolvedParameter = null;

            if (constructorParameter.ParameterType.IsAggregateReadModel())
            {
                if (!constructorParameter.ParameterType.GetTypeInfo().IsInterface)
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
                if (!constructorParameter.ParameterType.GetTypeInfo().IsInterface)
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
    }
}