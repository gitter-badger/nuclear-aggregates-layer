using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI.MassProcessing
{
    public sealed class IntegrationTestsMassProcessor : IMassProcessor
    {
        private static readonly Type IntegrationTestType = typeof(IIntegrationTest);

        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;
        private readonly Type[] _explicitlyTypesSpecified;
        private readonly Type[] _explicitlyExcludedTypes;
        private readonly List<Type> _testTypes = new List<Type>();

        public IntegrationTestsMassProcessor(
            IUnityContainer container,
            Func<LifetimeManager> lifetimeManagerFactoryMethod,
            Type[] explicitlyTypesSpecified,
            Type[] explicitlyExcludedTypes)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
            _explicitlyTypesSpecified = explicitlyTypesSpecified;
            _explicitlyExcludedTypes = explicitlyExcludedTypes;
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

            var testTypes =
              _explicitlyTypesSpecified != null && _explicitlyTypesSpecified.Any()
                  ? _testTypes.Intersect(_explicitlyTypesSpecified)
                  : _testTypes;

            testTypes =
                _explicitlyExcludedTypes != null && _explicitlyExcludedTypes.Any()
                    ? testTypes.Except(_explicitlyExcludedTypes)
                    : testTypes;

            foreach (var implementation in testTypes)
            {
                _container.RegisterOne2ManyTypesPerTypeUniqueness(IntegrationTestType, implementation, _lifetimeManagerFactoryMethod());
            }

            _container.RegisterType<ITestSuite, TestSuite>(Lifetime.Singleton, new InjectionConstructor(testTypes));
        }
    }
}