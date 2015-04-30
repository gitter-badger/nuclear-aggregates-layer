using System;

using DoubleGis.Erm.Platform.DI.Factories;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Config
{
    public static class InjectionFactories
    {
        public static InjectionFactory SimplifiedModelConsumer
        {
            get { return new InjectionFactory(SimplifiedModelConsumerInjectionFactory); }
        }

        public static InjectionFactory SimplifiedModelConsumerReadModel
        {
            get { return new InjectionFactory(SimplifiedModelConsumerReadModelInjectionFactory); }
        }

        public static InjectionFactory PersistenceService
        {
            get { return new InjectionFactory(PersistenceServiceInjectionFactory); }
        }

        private static object SimplifiedModelConsumerInjectionFactory(IUnityContainer container, Type consumerType, string registrationName)
        {
            var factory = container.Resolve<UnityRuntimeFactory>();
            return factory.CreateConsumer(consumerType);
        }

        private static object SimplifiedModelConsumerReadModelInjectionFactory(IUnityContainer container, Type consumerType, string registrationName)
        {
            var factory = container.Resolve<UnityRuntimeFactory>();
            return factory.CreateConsumerReadModel(consumerType);
        }

        private static object PersistenceServiceInjectionFactory(IUnityContainer container, Type persistenceServiceType, string registrationName)
        {
            var factory = container.Resolve<UnityRuntimeFactory>();
            return factory.CreatePersistenceService(persistenceServiceType);
        }
    }
}
