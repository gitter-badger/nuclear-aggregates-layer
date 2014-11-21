using System;

using DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions
{
    public static class PerScopeLifetimeBehaviorUtils
    {
        private const bool IsDisposableExtensionEnabledDefaultValue = false;

        public static void ExecuteScope(this IUnityContainer registrarContainer, Action<IUnityContainer> scopedAction, bool isDisposableExtensionEnabled)
        {
            using (var scopeContainer = registrarContainer.CreateChildContainer())
            {
                if (isDisposableExtensionEnabled)
                {
                    scopeContainer.AddExtension(new DisposableStrategyExtension());
                }
            
                scopedAction(scopeContainer);
            }
        }

        public static void ExecuteScope(this IUnityContainer registrarContainer, Action<IUnityContainer> scopedAction)
        {
            registrarContainer.ExecuteScope(scopedAction, IsDisposableExtensionEnabledDefaultValue);
        }

        public static TResult ExecuteScope<TResult>(this IUnityContainer registrarContainer, Func<IUnityContainer, TResult> scopedFunc, bool isDisposableExtensionEnabled)
        {
            using (var scopeContainer = registrarContainer.CreateChildContainer())
            {
                if (isDisposableExtensionEnabled)
                {
                    scopeContainer.AddExtension(new DisposableStrategyExtension());
                }

                return scopedFunc(scopeContainer);
            }
        }

        public static TResult ExecuteScope<TResult>(this IUnityContainer registrarContainer, Func<IUnityContainer, TResult> scopedFunc)
        {
            return registrarContainer.ExecuteScope(scopedFunc, IsDisposableExtensionEnabledDefaultValue);
        }
    }
}
