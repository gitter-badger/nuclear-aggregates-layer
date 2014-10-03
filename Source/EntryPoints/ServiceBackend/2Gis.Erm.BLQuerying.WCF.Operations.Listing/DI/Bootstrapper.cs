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
            container.RegisterType<IExtendedInfoFilterMetadata, UnityExtendedInfoFilterMetadata>(Lifetime.Singleton);
            container.RegisterType<IQdsExtendedInfoFilterMetadata, UnityQdsExtendedInfoFilterMetadata>(Lifetime.Singleton);

            return container;
        }
    }
}