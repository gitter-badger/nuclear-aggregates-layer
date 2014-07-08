using System;

using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Config
{
    public static class InjectionFactories
    {
        public static InjectionFactory SimplifiedModelConsumer
        {
            get { return new InjectionFactory(SimplifiedModelConsumerInjectionFactory); }
        }

        private static object SimplifiedModelConsumerInjectionFactory(IUnityContainer container, Type consumerType, string registrationName)
        {
            var factory = container.Resolve<ISimplifiedModelConsumerRuntimeFactory>();
            return factory.CreateConsumer(consumerType);
        }
    }
}
