using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.Model.Simplified;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Config.MassProcessing
{
    public class SimplifiedModelConsumersProcessor : IMassProcessor
    {
        private static readonly Type SimplifiedModelConsumerType = typeof(ISimplifiedModelConsumer);

        private readonly IUnityContainer _container;
        private readonly List<Type> _simplifiedModelConsumersTypes = new List<Type>();

        public SimplifiedModelConsumersProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { SimplifiedModelConsumerType };
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
                    .Where(i => SimplifiedModelConsumerType != i && SimplifiedModelConsumerType.IsAssignableFrom(i))
                    .ToArray();

                if (consumerInterfaces.Length == 0)
                {
                    continue;
                }

                foreach (var consumerInterface in consumerInterfaces)
                {
                    var genericArguments = consumerInterface.GetGenericArguments();
                    if (consumerInterface.IsGenericType && !consumerInterface.IsGenericTypeDefinition
                        && genericArguments.Any(x => x.IsGenericParameter))
                    {
                        // open generic
                        _container.RegisterType(
                                consumerInterface.GetGenericTypeDefinition(),
                                consumerImplementation,
                                Mapping.SimplifiedModelConsumerScope,
                                Lifetime.PerResolve,
                                new InjectionFactory(SimplifiedModelConsumerInjectionFactory));
                    }
                        
                    // closed generic
                    _container.RegisterType(
                        consumerInterface,
                        consumerImplementation,
                        Mapping.SimplifiedModelConsumerScope,
                        Lifetime.PerResolve,
                        new InjectionFactory(SimplifiedModelConsumerInjectionFactory));
                }
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

        private static object SimplifiedModelConsumerInjectionFactory(IUnityContainer container, Type consumerType, string registrationName)
        {
            var factory = container.Resolve<ISimplifiedModelConsumerRuntimeFactory>();
            return factory.CreateConsumer(consumerType);
        }
    }
}
