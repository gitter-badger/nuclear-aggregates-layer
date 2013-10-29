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
        {   // пока Resolve делается без указания имени, т.к. PublicService зарегистрирован через простой (неименованный) mapping
            // сделано это потому, что пока все handler помеченные ServiceLayerClientAttribute относятся к бизнесс логике Erm (не security, и не configuration)
            // => нет необходимости поддерживать несколько PublicService, каждая из которых косвенно работает только со своим entitymetadaworkspace
            // Т.о. пока в wcfservice нет поддержки аналога areas из web приложения
            // Если понадобиться все таки доступ к несколким регистрациям PublicService, тогда есть несколько вариантов:
            // 1). Из message выудить какой именно request был вызван и поддерживать карту соответсвия request->requesthandler->publicservice
            //      и при resolve использовать эти знания для получения нужной версии PublicService
            // 2). Просто сделать несколькоразных endpoint, т.е. для Erm request - свой (например, PublicService), а для других request свои endpoint
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
