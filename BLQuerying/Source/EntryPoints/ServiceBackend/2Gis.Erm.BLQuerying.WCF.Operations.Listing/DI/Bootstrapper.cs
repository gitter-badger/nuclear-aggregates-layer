using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Qds.Operations.Metadata;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.WCF.Operations.Listing.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureQdsListing(this IUnityContainer container)
        {
            return container
                .RegisterType<IExtendedInfoFilterMetadata, UnityExtendedInfoFilterMetadata>(Lifetime.Singleton)
                .RegisterType<IQdsExtendedInfoFilterMetadata, UnityQdsExtendedInfoFilterMetadata>(Lifetime.Singleton);
        }
    }
}