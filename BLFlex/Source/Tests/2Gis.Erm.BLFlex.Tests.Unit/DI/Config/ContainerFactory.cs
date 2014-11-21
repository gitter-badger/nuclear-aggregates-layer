using DoubleGis.Erm.BLFlex.DI.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.DI.Config
{
    static class ContainerFactory
    {
        public sealed class ForChile : IContainerFactory
        {
            public IUnityContainer CreateAndConfigure()
            {
                return new UnityContainer().ConfigureChileSpecificNumberServices();
            }
        }

        public sealed class ForCyprus : IContainerFactory
        {
            public IUnityContainer CreateAndConfigure()
            {
                return new UnityContainer().ConfigureCyprusSpecificNumberServices();
            }
        }

        public sealed class ForCzech : IContainerFactory
        {
            public IUnityContainer CreateAndConfigure()
            {
                return new UnityContainer().ConfigureCzechSpecificNumberServices();
            }
        }

        public sealed class ForRussia : IContainerFactory
        {
            public IUnityContainer CreateAndConfigure()
            {
                return new UnityContainer().ConfigureRussiaSpecificNumberServices();
            }
        }
    }
}