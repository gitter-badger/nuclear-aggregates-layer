using System;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Metadata;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI.Config
{
    public static partial class QueryingBootstrapper
    {
        public static IUnityContainer ConfigureListing(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container
                .RegisterType<IQuerySettingsProvider, QuerySettingsProvider>(entryPointSpecificLifetimeManagerFactory());
        }
    }
}
