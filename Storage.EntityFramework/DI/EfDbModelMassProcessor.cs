using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.Storage.Core;

namespace NuClear.Storage.EntityFramework.DI
{
    public class EFDbModelMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly List<IEFDbModelConfiguration> _configurations = new List<IEFDbModelConfiguration>();
        private readonly List<IEFDbModelConvention> _conventions = new List<IEFDbModelConvention>();
        private static readonly Type EfDbModelConfigurationMarker = typeof(IEFDbModelConfiguration);
        private static readonly Type EfDbModelConventionMarker = typeof(IEFDbModelConvention);

        public EFDbModelMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { EfDbModelConfigurationMarker, EfDbModelConventionMarker };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                if (EfDbModelConfigurationMarker.IsAssignableFrom(type))
                {
                    _configurations.Add((IEFDbModelConfiguration)Activator.CreateInstance(type));
                }
                else if (EfDbModelConventionMarker.IsAssignableFrom(type))
                {
                    _conventions.Add((IEFDbModelConvention)Activator.CreateInstance(type));
                }
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                return;
            }

            var entityTypeToContainerNameMap = _configurations.ToDictionary(x => x.EntityType, x => x.ContainerName);
            var containerNameToConfigurationsMap = _configurations.GroupBy(x => x.ContainerName).ToDictionary(x => x.Key, x => x.AsEnumerable());
            var containerNameToConventionsMap = _conventions.GroupBy(x => x.ContainerName).ToDictionary(x => x.Key, x => x.AsEnumerable());
            var provider = new EFDbModelConfigurationsProvider(entityTypeToContainerNameMap, containerNameToConfigurationsMap, containerNameToConventionsMap);

            _container.RegisterInstance<IEFDbModelConfigurationsProvider>(provider)
                      .RegisterInstance<IEntityContainerNameResolver>(provider);
        }

        private static bool ShouldBeProcessed(Type type)
        {
            return !type.IsAbstract;
        }
    }
}