using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public static partial class ContainerUtils
    {
        public static TFrom ResolveOne2ManyTypesByType<TFrom, TTo>(this IUnityContainer container)
        {
            return container.Resolve<TFrom>(GetPerTypeUniqueMarker<TTo>());
        }
    }
}