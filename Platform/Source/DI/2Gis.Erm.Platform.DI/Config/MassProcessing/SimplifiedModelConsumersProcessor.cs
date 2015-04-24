using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing
{
    public class SimplifiedModelConsumersProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly List<Type> _simplifiedModelConsumersTypes = new List<Type>();

        public SimplifiedModelConsumersProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return ModelIndicators.Simplified.Group.All;
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            _simplifiedModelConsumersTypes.AddRange(types.Where(ShouldBeProcessed));
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            foreach (var consumerImplementation in _simplifiedModelConsumersTypes)
            {
                var consumerInterfaces = consumerImplementation.GetInterfaces()
                                                               .Where(i => (i.IsSimplifiedModelConsumer() || i.IsSimplifiedModelConsumerReadModel()) &&
                                                                           i != ModelIndicators.Simplified.SimplifiedModelConsumer &&
                                                                           i != ModelIndicators.Simplified.SimplifiedModelConsumerReadModel)
                                                               .ToArray();

                if (consumerInterfaces.Length == 0)
                {
                    continue;
                }

                foreach (var consumerInterface in consumerInterfaces)
                {
                    if (consumerInterface.IsSimplifiedModelConsumerReadModel())
                    {
                        _container.RegisterType(consumerInterface,
                                                consumerImplementation,
                                                Mapping.SimplifiedModelConsumerReadModelScope,
                                                Lifetime.PerResolve,
                                                new InjectionFactory(SimplifiedModelConsumerReadModelInjectionFactory));
                    }
                    else
                    {
                        var genericArguments = consumerInterface.GetGenericArguments();
                        if (consumerInterface.IsGenericType && !consumerInterface.IsGenericTypeDefinition && genericArguments.Any(x => x.IsGenericParameter))
                        {
                            // open generic
                            _container.RegisterType(consumerInterface.GetGenericTypeDefinition(),
                                                    consumerImplementation,
                                                    Mapping.SimplifiedModelConsumerScope,
                                                    Lifetime.PerResolve,
                                                    InjectionFactories.SimplifiedModelConsumer);
                        }

                        // closed generic
                        _container.RegisterType(consumerInterface,
                                                consumerImplementation,
                                                Mapping.SimplifiedModelConsumerScope,
                                                Lifetime.PerResolve,
                                                InjectionFactories.SimplifiedModelConsumer);
                    }
                }

                _container.RegisterTypeWithDependencies(consumerImplementation, Lifetime.PerResolve, null);
            }
        }

        private static bool ShouldBeProcessed(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                return false;
            }

            return true;
        }

        private static object SimplifiedModelConsumerReadModelInjectionFactory(IUnityContainer container, Type consumerType, string registrationName)
        {
            var factory = container.Resolve<ISimplifiedModelConsumerRuntimeFactory>();
            return factory.CreateAggregateReadModel(consumerType);
        }
    }
}
