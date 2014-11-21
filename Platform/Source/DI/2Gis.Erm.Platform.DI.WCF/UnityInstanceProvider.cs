using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public sealed class UnityInstanceProvider : IInstanceProvider
    {
        private readonly IUnityContainer _container;
        private readonly Type _serviceType;
        
        public UnityInstanceProvider(IUnityContainer container, Type serviceType)
        {
            _container = container;
            _serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {   
            return _container.Resolve(_serviceType);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            // Since we created the service instance, we need to dispose of it, if needed.
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
