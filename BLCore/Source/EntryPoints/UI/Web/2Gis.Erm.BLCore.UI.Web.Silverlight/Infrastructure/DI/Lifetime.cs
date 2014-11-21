using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.DI
{
    public static class Lifetime
    {
        public static LifetimeManager Singleton
        {
            get { return new ContainerControlledLifetimeManager(); }
        }

        public static LifetimeManager PerResolve
        {
            get { return new PerResolveLifetimeManager(); }
        }

        public static LifetimeManager PerThread
        {
            get { return new PerThreadLifetimeManager(); }
        }

        public static LifetimeManager Hierarchical
        {
            get { return new HierarchicalLifetimeManager(); }
        }

        public static LifetimeManager External
        {
            get { return new ExternallyControlledLifetimeManager(); }
        }

        public static LifetimeManager Transient
        {
            get { return new TransientLifetimeManager(); }
        }
    }
}
