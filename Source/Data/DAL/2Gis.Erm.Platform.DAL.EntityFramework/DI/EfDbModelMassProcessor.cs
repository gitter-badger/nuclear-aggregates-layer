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

        public EfDbModelMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { typeof(IEfDbModelConfiguration) };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                var configuration = (IEfDbModelConfiguration)Activator.CreateInstance(type);
                _configurations.Add(configuration);
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                return;
            }

            // FIXME {a.tukaev, 27.10.2014}: Validate

            var entityTypeToContainerNameMap = _configurations.ToDictionary(x => x.EntityType, x => x.ContainerName);
            var containerNameToConfigurationsMap = _configurations.GroupBy(x => x.ContainerName).ToDictionary(x => x.Key, x => x.AsEnumerable());
            var provider = new EfDbModelConfigurationsProvider(entityTypeToContainerNameMap, containerNameToConfigurationsMap);

            _container.RegisterInstance<IEfDbModelConfigurationsProvider>(provider)
                      .RegisterInstance<IEntityContainerNameResolver>(provider);
        }

        private static bool ShouldBeProcessed(Type type)
        {
            return !type.IsAbstract;
        }
    }
}