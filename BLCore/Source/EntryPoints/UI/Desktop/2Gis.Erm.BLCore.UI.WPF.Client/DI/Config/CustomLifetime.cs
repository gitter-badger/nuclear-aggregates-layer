using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.Config
{
    public sealed class CustomLifetime
    {
        public static LifetimeManager PerUseCase
        {
            get { return new HierarchicalLifetimeManager(); }
        }
    }
}
