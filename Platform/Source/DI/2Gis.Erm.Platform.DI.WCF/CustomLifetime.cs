using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public static class CustomLifetime
    {
        public static LifetimeManager PerOperationContext
        {
            get { return new UnityOperationContextLifetimeManager(); }
        }
    }
}
