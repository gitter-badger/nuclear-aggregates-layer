using System;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public static partial class ContainerUtils
    {
        public static TFrom ResolveOne2ManyTypesByType<TFrom, TTo>(this IUnityContainer container)
            where TTo : TFrom
        {
            return container.Resolve<TFrom>(GetPerTypeUniqueMarker<TTo>());
        }

        public static TFrom ResolveOne2ManyTypesByType<TFrom>(this IUnityContainer container, Type to)
        {
            return container.Resolve<TFrom>(to.GetPerTypeUniqueMarker());
        }
    }
}