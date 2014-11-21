using System;
using System.ServiceModel.Dispatcher;

using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public class UnityInstanceProviderFactory : IInstanceProviderFactory
    {
        private readonly IUnityContainer _container;

        public UnityInstanceProviderFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IInstanceProvider Create(Type serviceType)
        {
            return new UnityInstanceProvider(_container, serviceType);
        }
    }
}