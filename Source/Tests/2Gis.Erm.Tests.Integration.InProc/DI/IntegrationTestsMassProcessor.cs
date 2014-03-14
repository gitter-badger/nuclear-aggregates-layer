using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    public sealed class IntegrationTestsMassProcessor : IMassProcessor
    {
        private static readonly Type IntegrationTestType = typeof(IIntegrationTest);

        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;
        private readonly List<Type> _testTypes = new List<Type>();

        public IntegrationTestsMassProcessor(
            IUnityContainer container,
            Func<LifetimeManager> lifetimeManagerFactoryMethod)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { IntegrationTestType };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            _testTypes.AddRange(types.Where(type => !type.IsInterface && !type.IsAbstract));
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            foreach (var implementation in _testTypes)
            {
                _container.RegisterOne2ManyTypesPerTypeUniqueness(IntegrationTestType, implementation, _lifetimeManagerFactoryMethod());
            }

            _container.RegisterType<ITestSuite, TestSuite>(Lifetime.Singleton, new InjectionConstructor(_testTypes));
        }
    }
}