using System;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Proxies
{
    public abstract partial class UnityContainerScopeProxy<TProxiedContract> : IDisposable
        where TProxiedContract : class
    {
        protected readonly TProxiedContract ProxiedInstance;
        private readonly IUnityContainer _unityContainer;

        protected UnityContainerScopeProxy(IUnityContainer unityContainer, TProxiedContract proxiedInstance)
        {
            _unityContainer = unityContainer;
            ProxiedInstance = proxiedInstance;
        }
    }
}