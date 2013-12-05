using System;

using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class TestFactory : ITestFactory
    {
        private readonly IUnityContainer _container;

        public TestFactory(IUnityContainer container)
        {
            _container = container;
        }

        public TestScope Create(Type testType)
        {
            var testContainer = _container.CreateChildContainer();
            try
            {
                var resolvedTest = _container.ResolveOne2ManyTypesByType<IIntegrationTest>(testType);
                return new TestScope(resolvedTest, testContainer);
            }
            catch (Exception)
            {
                testContainer.Dispose();
                throw;
            }
        }
    }
}
