using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing
{
    public class PersistenceServicesMassProcessor : IMassProcessor
    {
        private static readonly Type PersistenceServiceType = typeof(IPersistenceService);

        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;
        private readonly List<Type> _persistenceServicesTypes = new List<Type>();

        public PersistenceServicesMassProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeManagerFactoryMethod)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { PersistenceServiceType };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            _persistenceServicesTypes.AddRange(types.Where(type => !type.IsInterface && !type.IsAbstract));
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            foreach (var implementation in _persistenceServicesTypes)
            {
                var serviceInterfaces = implementation.GetInterfaces()
                    .Where(i => PersistenceServiceType != i && PersistenceServiceType.IsAssignableFrom(i))
                    .ToArray();

                if (serviceInterfaces.Length == 0)
                {
                    continue;
                }

                foreach (var serviceInterface in serviceInterfaces)
                {
                    var genericArguments = serviceInterface.GetGenericArguments();
                    if (serviceInterface.IsGenericType && !serviceInterface.IsGenericTypeDefinition
                        && genericArguments.Any(x => x.IsGenericParameter))
                    {
                        // open generic
                        _container.RegisterType(serviceInterface.GetGenericTypeDefinition(),
                                                implementation,
                                                Mapping.PersistenceServiceScope,
                                                _lifetimeManagerFactoryMethod(),
                                                new InjectionFactory(PersistenceServiceInjectionFactory));
                    }

                    // closed generic
                    _container.RegisterType(serviceInterface,
                                            implementation,
                                            Mapping.PersistenceServiceScope,
                                            _lifetimeManagerFactoryMethod(), 
                                            new InjectionFactory(PersistenceServiceInjectionFactory));
                }
            }
        }

        private static object PersistenceServiceInjectionFactory(IUnityContainer container, Type persistenceServiceType, string registrationName)
        {
            var factory = container.Resolve<IPersistenceServiceRuntimeFactory>();
            return factory.CreatePersistenceService(persistenceServiceType);
        }
    }
}