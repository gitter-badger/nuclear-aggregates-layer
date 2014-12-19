using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.Model.EntityFramework;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework.DI
{
    public class EfDbModelMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly List<IEfDbModelConfiguration> _configurations = new List<IEfDbModelConfiguration>();
        private readonly List<IEfDbModelConvention> _conventions = new List<IEfDbModelConvention>();
        private static readonly Type EfDbModelConfigurationMarker = typeof(IEfDbModelConfiguration);
        private static readonly Type EfDbModelConventionMarker = typeof(IEfDbModelConvention);

        public EfDbModelMassProcessor(IUnityContainer container)
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
                    _configurations.Add((IEfDbModelConfiguration)Activator.CreateInstance(type));
                }
                else if (EfDbModelConventionMarker.IsAssignableFrom(type))
                {
                    _conventions.Add((IEfDbModelConvention)Activator.CreateInstance(type));
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
            var provider = new EfDbModelConfigurationsProvider(entityTypeToContainerNameMap, containerNameToConfigurationsMap, containerNameToConventionsMap);

            _container.RegisterInstance<IEfDbModelConfigurationsProvider>(provider)
                      .RegisterInstance<IEntityContainerNameResolver>(provider);
        }

        private static bool ShouldBeProcessed(Type type)
        {
            return !type.IsAbstract;
        }
    }
}